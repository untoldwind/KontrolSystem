using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ArrayType : RealizedType {
        public readonly TO2Type elementType;
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public ArrayType(TO2Type _elementType) {
            elementType = _elementType;
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"set", new ArraySetFactory(this)},
                {"map", new ArrayMapFactory(this)},
                {"map_with_index", new ArrayMapWithIndexFactory(this)},
                {"to_string", new ArrayToStringFactory(this)},
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                {"length", new InlineFieldAccessFactory("Length of the array, i.e. number of elements in the array.", () => BuildinType.Int, OpCodes.Ldlen, OpCodes.Conv_I8) }
            };
        }

        public override string Name => $"{elementType}[]";

        public override bool IsValid(ModuleContext context) => elementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) => new ArrayType(elementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => elementType.GeneratedType(context).MakeArrayType();

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) {
            switch (indexSpec.indexType) {
            case IndexSpecType.Single:
                RealizedType underlyingElement = elementType.UnderlyingType(context);
                OpCode loadOp = OpCodes.Ldelem;

                return new InlineArrayIndexAccessEmitter(underlyingElement, BuildinType.Int, indexSpec.start);
            default:
                return null;
            }
        }

        public override IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => new ArrayForInSource(GeneratedType(context), elementType.UnderlyingType(context));
    }

    public class ArrayForInSource : IForInSource {
        private readonly Type arrayType;
        private readonly RealizedType elementType;

        private ILocalRef currentIndex;
        private ILocalRef arrayRef;

        public ArrayForInSource(Type _arrayType, RealizedType _elementType) {
            arrayType = _arrayType;
            elementType = _elementType;
        }

        public RealizedType ElementType => elementType;

        public void EmitInitialize(IBlockContext context) {
            arrayRef = context.DeclareHiddenLocal(arrayType);
            arrayRef.EmitStore(context);
            currentIndex = context.DeclareHiddenLocal(typeof(int));
            context.IL.Emit(OpCodes.Ldc_I4_M1);
            currentIndex.EmitStore(context);
        }

        public void EmitCheckDone(IBlockContext context, LabelRef loop) {
            currentIndex.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Add);
            context.IL.Emit(OpCodes.Dup);
            currentIndex.EmitStore(context);
            arrayRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldlen);
            context.IL.Emit(OpCodes.Conv_I4);
            context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
        }

        public void EmitNext(IBlockContext context) {
            arrayRef.EmitLoad(context);
            currentIndex.EmitLoad(context);
            if (elementType == BuildinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
            else if (elementType == BuildinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
            else if (elementType == BuildinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
            else context.IL.Emit(OpCodes.Ldelem, elementType.GeneratedType(context.ModuleContext));
        }

        public void EmitFinalize(IBlockContext context) { }
    }

    internal class ArraySetFactory : IMethodInvokeFactory {
        private readonly ArrayType arrayType;

        internal ArraySetFactory(ArrayType _arrayType) => arrayType = _arrayType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) {
            switch (argumentIdx) {
            case 0: return _ => BuildinType.Int;
            case 1: return context => arrayType.elementType.UnderlyingType(context.ModuleContext);
            default: return null;
            }
        }

        public string Description => "Set the element of an array";

        public TO2Type DeclaredReturn => BuildinType.Unit;

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("index", BuildinType.Int), new FunctionParameter("element", arrayType.elementType) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 2) return null;

            MethodInfo methodInfo = typeof(ArrayMethods).GetMethod("Set").MakeGenericMethod(arrayType.elementType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(BuildinType.Unit, new List<RealizedParameter> { new RealizedParameter("index", BuildinType.Int), new RealizedParameter("element", arrayType.elementType.UnderlyingType(context)) }, false, typeof(ArrayMethods), methodInfo);
        }
    }

    internal class ArrayMapFactory : IMethodInvokeFactory {
        private readonly ArrayType arrayType;

        internal ArrayMapFactory(ArrayType _arrayType) => arrayType = _arrayType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ => argumentIdx == 0 ? new FunctionType(false, new List<TO2Type> { arrayType.elementType }, BuildinType.Unit) : null;

        public string Description => "Map the content of the array";

        public TO2Type DeclaredReturn => new ArrayType(BuildinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("mapper", new FunctionType(false, new List<TO2Type> { arrayType.elementType }, BuildinType.Unit)) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context) as FunctionType;
            if (mapper == null || mapper.returnType == BuildinType.Unit) return null;

            MethodInfo methodInfo = typeof(ArrayMethods).GetMethod("Map").MakeGenericMethod(arrayType.elementType.GeneratedType(context), mapper.returnType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(new ArrayType(mapper.returnType), new List<RealizedParameter> { new RealizedParameter("mapper", mapper) }, false, typeof(ArrayMethods), methodInfo);
        }
    }

    internal class ArrayMapWithIndexFactory : IMethodInvokeFactory {
        private readonly ArrayType arrayType;

        internal ArrayMapWithIndexFactory(ArrayType _arrayType) => arrayType = _arrayType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ => argumentIdx == 0 ? new FunctionType(false, new List<TO2Type> { arrayType.elementType, BuildinType.Int }, BuildinType.Unit) : null;

        public string Description => "Map the content of the array";

        public TO2Type DeclaredReturn => new ArrayType(BuildinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("mapper", new FunctionType(false, new List<TO2Type> { arrayType.elementType, BuildinType.Int }, BuildinType.Unit)) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context) as FunctionType;
            if (mapper == null || mapper.returnType == BuildinType.Unit) return null;

            MethodInfo methodInfo = typeof(ArrayMethods).GetMethod("MapWithIndex").MakeGenericMethod(arrayType.elementType.GeneratedType(context), mapper.returnType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(new ArrayType(mapper.returnType), new List<RealizedParameter> { new RealizedParameter("mapper", mapper) }, false, typeof(ArrayMethods), methodInfo);
        }
    }

    internal class ArrayToStringFactory : IMethodInvokeFactory {
        private readonly ArrayType arrayType;

        internal ArrayToStringFactory(ArrayType _arrayType) => arrayType = _arrayType;

        public TypeHint ReturnHint => _ => BuildinType.String;

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public string Description => "String representation of the array";

        public TO2Type DeclaredReturn => BuildinType.String;

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 0) return null;

            MethodInfo methodInfo = typeof(ArrayMethods).GetMethod("ArrayToString").MakeGenericMethod(arrayType.elementType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(BuildinType.String, new List<RealizedParameter> { }, false, typeof(ArrayMethods), methodInfo);
        }
    }
}
