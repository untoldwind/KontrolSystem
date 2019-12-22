using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class BinaryBool : Expression {
        public readonly Operator op;
        public readonly Expression left;
        public readonly Expression right;

        public BinaryBool(Expression _left, Operator _op, Expression _right, Position start = new Position(), Position end = new Position()) : base(start, end) {
            left = _left;
            op = _op;
            right = _right;
            left.SetTypeHint(_ => BuildinType.Bool);
            right.SetTypeHint(_ => BuildinType.Bool);
        }

        public override void SetVariableContainer(IVariableContainer container) {
            left.SetVariableContainer(container);
            right.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Bool;

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            if (leftType != BuildinType.Bool)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    "Expected boolean",
                    left.Start,
                    left.End
                ));
            if (rightType != BuildinType.Bool)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    "Expected boolean",
                    right.Start,
                    right.End
                ));

            if (context.HasErrors) return;

            left.EmitCode(context, false);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);

            ILCount rightCount = right.GetILCount(context, dropResult);
            LabelRef skipRight = context.IL.DefineLabel(rightCount.opCodes < 124);

            if (context.HasErrors) return;

            switch (op) {
            case Operator.BoolAnd:
                context.IL.Emit(skipRight.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipRight);
                break;
            case Operator.BoolOr:
                context.IL.Emit(skipRight.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, skipRight);
                break;
            default:
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidOperator,
                    $"Invalid boolean operator {op}",
                    Start,
                    End
                ));
                return;
            }

            if (!dropResult) context.IL.Emit(OpCodes.Pop);

            right.EmitCode(context, dropResult);
            context.IL.MarkLabel(skipRight);
        }
    }
}
