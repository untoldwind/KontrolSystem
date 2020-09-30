using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public interface IFieldAccessEmitter {
        RealizedType FieldType { get; }

        bool RequiresPtr { get; }

        void EmitLoad(IBlockContext context);
    }

    public interface IFieldAccessFactory {
        TO2Type DeclaredType { get; }

        string Description { get; }

        IFieldAccessEmitter Create(ModuleContext context);

        IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
    }

    public class InlineFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly OpCode[] opCodes;
        private readonly string description;

        public InlineFieldAccessFactory(string description, Func<RealizedType> fieldType, params OpCode[] opCodes) {
            this.description = description;
            this.fieldType = fieldType;
            this.opCodes = opCodes;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) => new InlineFieldAccessEmitter(fieldType(), opCodes);

        public IFieldAccessFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    public class InlineFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly OpCode[] opCodes;

        public InlineFieldAccessEmitter(RealizedType fieldType, OpCode[] opCodes) {
            this.fieldType = fieldType;
            this.opCodes = opCodes;
        }

        public RealizedType FieldType => fieldType;

        public bool RequiresPtr => false;

        public void EmitLoad(IBlockContext context) {
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }
    }

    public class BoundFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly List<FieldInfo> fieldInfos;
        private readonly Type fieldTarget;
        private readonly string description;

        public BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
            FieldInfo fieldInfo) {
            this.description = description;
            this.fieldType = fieldType;
            this.fieldTarget = fieldTarget;
            fieldInfos = new List<FieldInfo> {fieldInfo};
        }

        public BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
            List<FieldInfo> fieldInfos) {
            this.description = description;
            this.fieldType = fieldType;
            this.fieldTarget = fieldTarget;
            this.fieldInfos = fieldInfos;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) =>
            new BoundFieldAccessEmitter(fieldType(), fieldTarget, fieldInfos);

        public BoundFieldAccessFactory Prefix(FieldInfo nextField) {
            List<FieldInfo> next = nextField.Yield().Concat(fieldInfos).ToList();

            return new BoundFieldAccessFactory(description, fieldType, fieldTarget, next);
        }

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (fieldTarget.IsGenericTypeDefinition) {
                Type[] arguments = fieldTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = fieldTarget.MakeGenericType(arguments);
                List<FieldInfo> genericFields = new List<FieldInfo>();
                Type current = genericTarget;

                foreach (FieldInfo field in fieldInfos) {
                    FieldInfo genericField = current.GetField(field.Name);
                    genericFields.Add(genericField);
                    current = genericField.FieldType;
                }

                return new BoundFieldAccessFactory(description, () => fieldType().FillGenerics(context, typeArguments),
                    genericTarget, genericFields);
            }

            return this;
        }
    }

    public class BoundFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly List<FieldInfo> fieldInfos;
        private readonly Type fieldTarget;

        public BoundFieldAccessEmitter(RealizedType fieldType, Type fieldTarget, List<FieldInfo> fieldInfos) {
            this.fieldType = fieldType;
            this.fieldTarget = fieldTarget;
            this.fieldInfos = fieldInfos;
        }

        public RealizedType FieldType => fieldType;

        public bool RequiresPtr => fieldTarget.IsValueType;

        public void EmitLoad(IBlockContext context) {
            foreach (FieldInfo fieldInfo in fieldInfos)
                context.IL.Emit(OpCodes.Ldfld, fieldInfo);
        }
    }

    public class BoundPropertyLikeFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly MethodInfo getter;
        private readonly OpCode[] opCodes;
        private readonly Type methodTarget;
        private readonly string description;

        public BoundPropertyLikeFieldAccessFactory(string description, Func<RealizedType> fieldType, Type methodTarget,
            MethodInfo getter, params OpCode[] opCodes) {
            this.description = description;
            this.fieldType = fieldType;
            this.methodTarget = methodTarget;
            this.getter = getter;
            this.opCodes = opCodes;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) =>
            new BoundPropertyLikeFieldAccessEmitter(fieldType(), methodTarget, getter, opCodes);

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (methodTarget.IsGenericTypeDefinition) {
                Type[] arguments = methodTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = methodTarget.MakeGenericType(arguments);
                MethodInfo genericMethod = genericTarget.GetMethod(getter.Name, new Type[0]);

                if (genericMethod == null)
                    throw new ArgumentException(
                        $"Unable to relocate method {getter.Name} on {methodTarget} for type arguments {typeArguments}");

                return new BoundPropertyLikeFieldAccessFactory(description,
                    () => fieldType().FillGenerics(context, typeArguments), genericTarget, genericMethod, opCodes);
            }

            return this;
        }
    }

    public class BoundPropertyLikeFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly MethodInfo getter;
        private readonly OpCode[] opCodes;
        private readonly Type methodTarget;

        public BoundPropertyLikeFieldAccessEmitter(RealizedType fieldType, Type methodTarget, MethodInfo getter,
            OpCode[] opCodes) {
            this.fieldType = fieldType;
            this.methodTarget = methodTarget;
            this.getter = getter;
            this.opCodes = opCodes;
        }

        public RealizedType FieldType => fieldType;

        public bool RequiresPtr =>
            methodTarget.IsValueType && (getter.CallingConvention & CallingConventions.HasThis) != 0;

        public void EmitLoad(IBlockContext context) {
            if (getter.IsVirtual) context.IL.EmitCall(OpCodes.Callvirt, getter, 1);
            else context.IL.EmitCall(OpCodes.Call, getter, 1);
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }
    }
}
