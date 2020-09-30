using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class IfThen : Expression {
        public readonly Expression condition;
        public readonly Expression thenExpression;

        public IfThen(Expression condition, Expression thenExpression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.condition = condition;
            this.condition.SetTypeHint(_ => BuiltinType.Bool);
            this.thenExpression = thenExpression;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            condition.SetVariableContainer(container);
            thenExpression.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) => thenExpression.SetTypeHint(context =>
            (typeHint(context) as OptionType)?.elementType.UnderlyingType(context.ModuleContext));

        public override TO2Type ResultType(IBlockContext context) => new OptionType(thenExpression.ResultType(context));

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            ILCount thenCount = thenExpression.GetILCount(context, true);

            if (!context.HasErrors && thenCount.stack > 0) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        $"Then expression leaves values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            condition.EmitCode(context, false);

            if (context.HasErrors) return;

            if (condition.ResultType(context) != BuiltinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Condition of if is not a boolean",
                        Start,
                        End
                    )
                );
                return;
            }

            TO2Type thenResultType = thenExpression.ResultType(context);

            if (dropResult) {
                LabelRef skipThen = context.IL.DefineLabel(thenCount.opCodes < 124);

                if (!dropResult) context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
                thenExpression.EmitCode(context, true);
                context.IL.MarkLabel(skipThen);
            } else {
                OptionType optionType = new OptionType(thenResultType);
                Type generatedType = optionType.GeneratedType(context.ModuleContext);
                ILocalRef tempResult = context.IL.TempLocal(generatedType);
                LabelRef skipThen = context.IL.DefineLabel(thenCount.opCodes < 114);

                context.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
                tempResult.EmitLoadPtr(context);
                context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
                context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(OpCodes.Ldc_I4_1);
                context.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
                thenExpression.EmitCode(context, false);
                context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
                LabelRef ifEnd = context.IL.DefineLabel(true);
                context.IL.Emit(OpCodes.Br_S, ifEnd);

                context.IL.MarkLabel(skipThen);

                tempResult.EmitLoadPtr(context);
                context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);

                context.IL.MarkLabel(ifEnd);

                tempResult.EmitLoad(context);
            }
        }
    }

    public class IfThenElse : Expression {
        public readonly Expression condition;
        public readonly Expression thenExpression;
        public readonly Expression elseExpression;

        public IfThenElse(Expression condition, Expression thenExpression, Expression elseExpression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.condition = condition;
            this.thenExpression = thenExpression;
            this.elseExpression = elseExpression;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            condition.SetVariableContainer(container);
            thenExpression.SetVariableContainer(container);
            elseExpression.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) {
            condition.SetTypeHint(_ => BuiltinType.Bool);
            thenExpression.SetTypeHint(typeHint);
            elseExpression.SetTypeHint(typeHint);
        }


        public override TO2Type ResultType(IBlockContext context) => thenExpression.ResultType(context);

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            ILCount thenCount = thenExpression.GetILCount(context, dropResult);
            ILCount elseCount = elseExpression.GetILCount(context, dropResult);

            if (!context.HasErrors && thenCount.stack > 1) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        $"Then expression leaves too many values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            if (!context.HasErrors && elseCount.stack > 1) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        $"Else expression leaves too many values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            condition.EmitCode(context, false);

            if (context.HasErrors) return;

            if (condition.ResultType(context) != BuiltinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Condition of if is not a boolean",
                        Start,
                        End
                    )
                );
                return;
            }

            TO2Type thenType = thenExpression.ResultType(context);
            TO2Type elseType = elseExpression.ResultType(context);
            if (!dropResult) {
                if (!thenType.IsAssignableFrom(context.ModuleContext, elseType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"If condition has incompatible result {thenType} != {elseType}",
                        Start,
                        End
                    ));
                }
            }

            if (context.HasErrors) return;

            LabelRef thenEnd = context.IL.DefineLabel(thenCount.opCodes < 124);
            LabelRef elseEnd = context.IL.DefineLabel(elseCount.opCodes < 124);

            context.IL.Emit(thenEnd.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, thenEnd);
            thenExpression.EmitCode(context, dropResult);
            context.IL.Emit(elseEnd.isShort ? OpCodes.Br_S : OpCodes.Br, elseEnd);
            context.IL.MarkLabel(thenEnd);
            elseExpression.EmitCode(context, dropResult);
            if (!dropResult) thenType.AssignFrom(context.ModuleContext, elseType).EmitConvert(context);
            context.IL.MarkLabel(elseEnd);
            if (thenCount.stack > 0 && elseCount.stack > 0) context.IL.AdjustStack(-1);
        }
    }
}
