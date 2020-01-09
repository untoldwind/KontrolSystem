using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ArrayBuilderType : RealizedType {
        public readonly TO2Type elementType;
        private Type generatedType;
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;
        private OperatorCollection allowedSuffixOperators;

        public ArrayBuilderType(TO2Type _elementType) {
            elementType = _elementType;
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"append", new ArrayBuilderAppendFactory(this) },
                {"result", new ArrayBuilderResultFactory(this) },
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                {"length", new ArrayBuilderLengthFactory(this) },
            };
            allowedSuffixOperators = new OperatorCollection {
                {Operator.AddAssign, new ArrayBuilderAppendFactory(this)}
            };
        }

        public override string Name => $"ArrayBuilder<{elementType}>";

        public override bool IsValid(ModuleContext context) => elementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) => new ArrayBuilderType(elementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = DeriveType(context));

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => new ArrayBuilderType(elementType.UnderlyingType(context).FillGenerics(context, typeArguments));

        private Type DeriveType(ModuleContext context) => (elementType == BuildinType.Unit) ? typeof(bool) : typeof(ArrayBuilder<>).MakeGenericType(elementType.GeneratedType(context));
    }

    internal class ArrayBuilderAppendFactory : IMethodInvokeFactory, IOperatorEmitter {
        private readonly ArrayBuilderType arrayBuilderType;

        internal ArrayBuilderAppendFactory(ArrayBuilderType _arrayBuilderType) => arrayBuilderType = _arrayBuilderType;

        public TypeHint ReturnHint => _ => arrayBuilderType;

        public TypeHint ArgumentHint(int argumentIdx) => context => argumentIdx == 0 ? arrayBuilderType.elementType.UnderlyingType(context.ModuleContext) : null;

        public string Description => "Append new element to the array";

        public TO2Type DeclaredReturn => arrayBuilderType;

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("element", arrayBuilderType.elementType) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            if (!arrayBuilderType.elementType.IsAssignableFrom(context, arguments[0])) return null;

            Type generatedType = arrayBuilderType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetMethod("Append");

            return new BoundMethodInvokeEmitter(arrayBuilderType, new List<RealizedParameter> { new RealizedParameter("element", arrayBuilderType.elementType.UnderlyingType(context)) }, false, generatedType, methodInfo);
        }

        public TO2Type ResultType => arrayBuilderType;

        public TO2Type OtherType => arrayBuilderType.elementType;

        public bool Accepts(ModuleContext context, TO2Type otherType) => arrayBuilderType.elementType.IsAssignableFrom(context, otherType);

        public void EmitCode(IBlockContext context, Node target) {
            Type generatedType = arrayBuilderType.GeneratedType(context.ModuleContext);
            MethodInfo methodInfo = generatedType.GetMethod("Append");

            context.IL.EmitCall(OpCodes.Call, methodInfo, 2);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            context.IL.Emit(OpCodes.Pop);
        }

        public IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class ArrayBuilderResultFactory : IMethodInvokeFactory {
        private readonly ArrayBuilderType arrayBuilderType;

        internal ArrayBuilderResultFactory(ArrayBuilderType _arrayBuilderType) => arrayBuilderType = _arrayBuilderType;

        public TypeHint ReturnHint => _ => new ArrayType(arrayBuilderType.elementType);

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public string Description => "Append new element to the array";

        public TO2Type DeclaredReturn => new ArrayType(arrayBuilderType.elementType);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 0) return null;

            Type generatedType = arrayBuilderType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetMethod("Result");

            return new BoundMethodInvokeEmitter(new ArrayType(arrayBuilderType.elementType), new List<RealizedParameter> { }, false, generatedType, methodInfo);
        }

        public IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class ArrayBuilderLengthFactory : IFieldAccessFactory {
        private readonly ArrayBuilderType arrayBuilderType;

        internal ArrayBuilderLengthFactory(ArrayBuilderType _arrayBuilderType) => arrayBuilderType = _arrayBuilderType;

        public TO2Type DeclaredType => BuildinType.Int;

        public string Description => "Get current length of array";

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generatedType = arrayBuilderType.GeneratedType(context);

            return new BoundPropertyLikeFieldAccessEmitter(BuildinType.Int, generatedType, generatedType.GetProperty("Length").GetMethod, new OpCode[0]);
        }

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }
}
