using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class While : Expression {
        public readonly Expression condition;
        public readonly Expression loopExpression;

        public While(Expression _condition, Expression _loopExpression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            condition = _condition;
            condition.SetTypeHint(_ => BuildinType.Bool);
            loopExpression = _loopExpression;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            condition.SetVariableContainer(container);
            loopExpression.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public override void Prepare(IBlockContext context) { }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (condition.ResultType(context) != BuildinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Condition of while is not a boolean",
                        Start,
                        End
                    )
                );
            }

            IBlockContext tmpContext = context.CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            ILCount conditionCount = condition.GetILCount(tmpContext, false);
            ILCount loopCount = loopExpression.GetILCount(tmpContext, true);

            if (loopCount.stack > 0) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        $"Body of the while expression leaves values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            LabelRef whileStart = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 124);
            LabelRef whileEnd = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 124);
            LabelRef whileLoop = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 114);
            IBlockContext loopContext = context.CreateLoopContext(whileStart, whileEnd);

            if (context.HasErrors) return;

            loopContext.IL.Emit(whileStart.isShort ? OpCodes.Br_S : OpCodes.Br, whileStart);
            context.IL.MarkLabel(whileLoop);

            // Timeout check
            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldfld, context.ModuleContext.contextField);
            context.IL.EmitCall(OpCodes.Callvirt, typeof(KontrolSystem.TO2.Runtime.IContext).GetMethod("CheckTimeout"), 1);

            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(whileStart);
            condition.EmitCode(loopContext, false);

            loopContext.IL.Emit(whileLoop.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, whileLoop);

            loopContext.IL.MarkLabel(whileEnd);
        }
    }
}
