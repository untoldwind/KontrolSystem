using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public interface IFieldAccessEmitter {
        RealizedType FieldType {
            get;
        }

        bool RequiresPtr {
            get;
        }

        void EmitLoad(IBlockContext context);
    }

    public interface IFieldAccessFactory {
        TO2Type DeclaredType { get; }

        string Description { get; }

        IFieldAccessEmitter Create(ModuleContext context);
    }

    public class InlineFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly OpCode[] opCodes;
        private readonly string description;

        public InlineFieldAccessFactory(string _description, Func<RealizedType> _fieldType, params OpCode[] _opCodes) {
            description = _description;
            fieldType = _fieldType;
            opCodes = _opCodes;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) => new InlineFieldAccessEmitter(fieldType(), opCodes);
    }

    public class InlineFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly OpCode[] opCodes;

        public InlineFieldAccessEmitter(RealizedType _fieldType, OpCode[] _opCodes) {
            fieldType = _fieldType;
            opCodes = _opCodes;
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

        public BoundFieldAccessFactory(string _description, Func<RealizedType> _fieldType, Type _fieldTarget, FieldInfo _fieldInfo) {
            description = _description;
            fieldType = _fieldType;
            fieldTarget = _fieldTarget;
            fieldInfos = new List<FieldInfo> { _fieldInfo };
        }

        public BoundFieldAccessFactory(string _description, Func<RealizedType> _fieldType, Type _fieldTarget, List<FieldInfo> _fieldInfos) {
            description = _description;
            fieldType = _fieldType;
            fieldTarget = _fieldTarget;
            fieldInfos = _fieldInfos;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) => new BoundFieldAccessEmitter(fieldType(), fieldTarget, fieldInfos);

        public BoundFieldAccessFactory Prefix(FieldInfo nextField) {
            List<FieldInfo> next = nextField.Yield().Concat(fieldInfos).ToList();

            return new BoundFieldAccessFactory(description, fieldType, fieldTarget, next);
        }
    }

    public class BoundFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly List<FieldInfo> fieldInfos;
        private readonly Type fieldTarget;

        public BoundFieldAccessEmitter(RealizedType _fieldType, Type _fieldTarget, List<FieldInfo> _fieldInfos) {
            fieldType = _fieldType;
            fieldTarget = _fieldTarget;
            fieldInfos = _fieldInfos;
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

        public BoundPropertyLikeFieldAccessFactory(string _description, Func<RealizedType> _fieldType, Type _methodTarget, MethodInfo _getter, params OpCode[] _opCodes) {
            description = _description;
            fieldType = _fieldType;
            methodTarget = _methodTarget;
            getter = _getter;
            opCodes = _opCodes;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) => new BoundPropertyLikeFieldAccessEmitter(fieldType(), methodTarget, getter, opCodes);
    }

    public class BoundPropertyLikeFieldAccessEmitter : IFieldAccessEmitter {
        private readonly RealizedType fieldType;
        private readonly MethodInfo getter;
        private readonly OpCode[] opCodes;
        private readonly Type methodTarget;

        public BoundPropertyLikeFieldAccessEmitter(RealizedType _fieldType, Type _methodTarget, MethodInfo _getter, OpCode[] _opCodes) {
            fieldType = _fieldType;
            methodTarget = _methodTarget;
            getter = _getter;
            opCodes = _opCodes;
        }

        public RealizedType FieldType => fieldType;

        public bool RequiresPtr => methodTarget.IsValueType && (getter.CallingConvention & CallingConventions.HasThis) != 0;

        public void EmitLoad(IBlockContext context) {
            if (getter.IsVirtual) context.IL.EmitCall(OpCodes.Callvirt, getter, 1);
            else context.IL.EmitCall(OpCodes.Call, getter, 1);
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }
    }
}
