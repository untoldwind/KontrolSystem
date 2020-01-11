using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class ArrayType : RealizedType {
        public readonly TO2Type elementType;
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public ArrayType(TO2Type _elementType) {
            elementType = _elementType;
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"set", new BoundMethodInvokeFactory("Set/update an element of the array",
                                                     () => BuildinType.Unit,
                                                     () => new List<RealizedParameter> { new RealizedParameter("index", BuildinType.Int), new RealizedParameter("element", new GenericParameter("T")) },
                                                     false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Set"),
                                                     context => ("T", elementType.UnderlyingType(context)).Yield())},
                {"map", new BoundMethodInvokeFactory("Map the content of the array",
                                                     () => new ArrayType(new GenericParameter("U")),
                                                     () => new List<RealizedParameter> { new RealizedParameter("mapper", new FunctionType(false, new List<TO2Type> { elementType }, new GenericParameter("U"))) },
                                                     false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Map"),
                                                     context => ("T", elementType.UnderlyingType(context)).Yield())},
                {"map_with_index", new BoundMethodInvokeFactory("Map the content of the array",
                                                     () => new ArrayType(new GenericParameter("U")),
                                                     () => new List<RealizedParameter> { new RealizedParameter("mapper", new FunctionType(false, new List<TO2Type> { elementType, BuildinType.Int }, new GenericParameter("U"))) },
                                                     false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("MapWithIndex"),
                                                     context => ("T", elementType.UnderlyingType(context)).Yield())},
                {"to_string", new BoundMethodInvokeFactory("Get string representation of the array",
                                                     () => BuildinType.String,
                                                     () => new List<RealizedParameter> { },
                                                     false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("ArrayToString"),
                                                     context => ("T", elementType.UnderlyingType(context)).Yield())},
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

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => new ArrayType(elementType.UnderlyingType(context).FillGenerics(context, typeArguments));

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context, RealizedType concreteType) {
            ArrayType concreteArray = concreteType as ArrayType;
            if (concreteArray == null) return Enumerable.Empty<(string name, RealizedType type)>();
            return elementType.InferGenericArgument(context, concreteArray.elementType.UnderlyingType(context));
        }
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
    }
}
