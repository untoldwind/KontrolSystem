using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ResultType : RealizedType {
        public readonly TO2Type successType;
        public readonly TO2Type errorType;
        private Type generatedType;
        private OperatorCollection allowedSuffixOperators;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public ResultType(TO2Type _successType, TO2Type _errorType) {
            successType = _successType;
            errorType = _errorType;
            allowedSuffixOperators = new OperatorCollection {
                {Operator.Unwrap, new ResultUnwrapOperator(this) }
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"success", new ResultFieldAccess(this, ResultField.Success) },
                    {"value", new ResultFieldAccess(this, ResultField.Value)},
                    {"error", new ResultFieldAccess(this, ResultField.Error)}
                };
        }

        public override string Name => $"Result<{successType}, {errorType}>";

        public override bool IsValid(ModuleContext context) => successType.IsValid(context) && errorType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) => new ResultType(successType.UnderlyingType(context), errorType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = DeriveType(context));

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            ResultType otherResultType = otherType.UnderlyingType(context) as ResultType;

            if (otherResultType != null) return successType.IsAssignableFrom(context, otherResultType.successType) && errorType.IsAssignableFrom(context, otherResultType.errorType);

            return successType.IsAssignableFrom(context, otherType);
        }

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            RealizedType underlyingOther = otherType.UnderlyingType(context);
            return !(underlyingOther is ResultType) && successType.IsAssignableFrom(context, underlyingOther) ? new AssignOk(this, otherType) : DefaultAssignEmitter.Instance;
        }

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => new ResultType(successType.UnderlyingType(context).FillGenerics(context, typeArguments), errorType.UnderlyingType(context).FillGenerics(context, typeArguments));

        private Type DeriveType(ModuleContext context) => typeof(Result<,>).MakeGenericType(successType.GeneratedType(context), errorType.GeneratedType(context));
    }

    internal enum ResultField {
        Success,
        Value,
        Error,
    }

    internal class ResultFieldAccess : IFieldAccessFactory {
        private ResultType resultType;
        private ResultField field;

        internal ResultFieldAccess(ResultType _resultType, ResultField _field) {
            resultType = _resultType;
            field = _field;
        }

        public TO2Type DeclaredType {
            get {
                switch (field) {
                case ResultField.Success: return BuildinType.Bool;
                case ResultField.Value: return resultType.successType;
                case ResultField.Error: return resultType.errorType;
                default: throw new InvalidOperationException($"Unkown option field: {field}");
                }
            }
        }

        public string Description {
            get {
                switch (field) {
                case ResultField.Success: return "`true` if the operation was successful";
                case ResultField.Value: return "Successful result of the operation";
                case ResultField.Error: return "Error result of the operation";
                default: throw new InvalidOperationException($"Unkown option field: {field}");
                }
            }
        }

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = resultType.GeneratedType(context);
            switch (field) {
            case ResultField.Success: return new BoundFieldAccessEmitter(BuildinType.Bool, generateType, new List<FieldInfo> { generateType.GetField("success") });
            case ResultField.Value: return new BoundFieldAccessEmitter(resultType.successType.UnderlyingType(context), generateType, new List<FieldInfo> { generateType.GetField("value") });
            case ResultField.Error: return new BoundFieldAccessEmitter(resultType.errorType.UnderlyingType(context), generateType, new List<FieldInfo> { generateType.GetField("error") });
            default: throw new InvalidOperationException($"Unkown option field: {field}");
            }
        }

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class AssignOk : IAssignEmitter {
        private readonly ResultType resultType;
        private readonly TO2Type otherType;

        internal AssignOk(ResultType _resultType, TO2Type _otherType) {
            resultType = _resultType;
            otherType = _otherType;
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            IBlockVariable valueTemp = context.MakeTempVariable(resultType.successType.UnderlyingType(context.ModuleContext));

            resultType.successType.AssignFrom(context.ModuleContext, otherType).EmitAssign(context, valueTemp, expression, true);

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            valueTemp.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            if (!dropResult) variable.EmitLoad(context);
        }

        public void EmitConvert(IBlockContext context) {
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            ILocalRef value = context.IL.TempLocal(resultType.successType.GeneratedType(context.ModuleContext));

            resultType.successType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context);
            value.EmitStore(context);

            ILocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            value.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            someResult.EmitLoad(context);
        }
    }

    internal class ResultUnwrapOperator : IOperatorEmitter {
        private readonly ResultType resultType;

        internal ResultUnwrapOperator(ResultType _resultType) => resultType = _resultType;

        public bool Accepts(ModuleContext context, TO2Type _otherType) => _otherType == BuildinType.Unit;

        public TO2Type OtherType => BuildinType.Unit;

        public TO2Type ResultType => resultType.successType;

        public bool RequirePtr => false;

        public void EmitCode(IBlockContext context, Node target) {
            ResultType expectedReturn = context.ExpectedReturn.UnderlyingType(context.ModuleContext) as ResultType;
            if (expectedReturn == null || !expectedReturn.errorType.IsAssignableFrom(context.ModuleContext, resultType.errorType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Operator ? is only allowed if function returns a result with error type {resultType.errorType}",
                    target.Start,
                    target.End
                ));
                return;
            }

            // Take success
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));

            LabelRef onSuccess = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
            // Keep track of stuff that is still on the stack at onSuccess
            int stackAdjust = context.IL.StackCount;
            ILocalRef tempError = context.IL.TempLocal(expectedReturn.errorType.GeneratedType(context.ModuleContext));
            Type errorResultType = expectedReturn.GeneratedType(context.ModuleContext);
            ILocalRef errorResult = context.IL.TempLocal(errorResultType);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("error"));
            tempError.EmitStore(context);
            // Clean stack entirely to make room for error result to return
            for (int i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
            errorResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, errorResultType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_0);
            context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("success"));
            tempError.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("error"));
            errorResult.EmitLoad(context);
            if (context.IsAsync) {
                context.IL.EmitNew(OpCodes.Newobj, context.MethodBuilder.ReturnType.GetConstructor(new Type[] { errorResultType }));
            }
            context.IL.EmitReturn(context.MethodBuilder.ReturnType);

            context.IL.MarkLabel(onSuccess);

            // Readjust the stack counter
            context.IL.AdjustStack(stackAdjust);
            // Get success value if necessary or drop result
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }
}
