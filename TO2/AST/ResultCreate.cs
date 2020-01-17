using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ResultOkCreate : Expression {
        public readonly Expression expression;

        private TypeHint typeHint;

        public ResultOkCreate(Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            expression = _expression;
        }

        public override void SetVariableContainer(IVariableContainer container) => expression.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            expression.SetTypeHint(context => (typeHint?.Invoke(context) as ResultType)?.successType.UnderlyingType(context.ModuleContext));
        }

        public override TO2Type ResultType(IBlockContext context) {
            return typeHint?.Invoke(context) as ResultType ?? BuildinType.Unit;
        }

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            ResultType resultType = ResultType(context) as ResultType;

            if (resultType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of Result. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            Type generatedType = resultType.GeneratedType(context.ModuleContext);

            IBlockVariable tempVariable = context.MakeTempVariable(resultType);
            EmitStore(context, tempVariable, dropResult);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            Type generatedType = variable.Type.GeneratedType(context.ModuleContext);
            ResultType resultHint = variable.Type as ResultType;

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            expression.EmitCode(context, false);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            if (!dropResult) variable.EmitLoad(context);
        }
    }

    public class ResultErrCreate : Expression {
        public readonly Expression expression;

        private TypeHint typeHint;

        public ResultErrCreate(Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            expression = _expression;
        }

        public override void SetVariableContainer(IVariableContainer container) => expression.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            expression.SetTypeHint(context => (typeHint?.Invoke(context) as ResultType)?.errorType.UnderlyingType(context.ModuleContext));
        }

        public override TO2Type ResultType(IBlockContext context) {
            return typeHint?.Invoke(context) as ResultType ?? BuildinType.Unit;
        }

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            ResultType resultType = ResultType(context) as ResultType;

            if (resultType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of Result. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            Type generatedType = resultType.GeneratedType(context.ModuleContext);

            IBlockVariable tempVariable = context.MakeTempVariable(resultType);
            EmitStore(context, tempVariable, dropResult);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            Type generatedType = variable.Type.GeneratedType(context.ModuleContext);
            ResultType resultHint = variable.Type as ResultType;

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_0);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            expression.EmitCode(context, false);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("error"));
            if (!dropResult) variable.EmitLoad(context);
        }
    }
}
