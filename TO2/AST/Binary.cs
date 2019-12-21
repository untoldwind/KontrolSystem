using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class Binary : Expression {
        public readonly Operator op;
        public readonly Expression left;
        public readonly Expression right;

        public Binary(Expression _left, Operator _op, Expression _right, Position start = new Position(), Position end = new Position()) : base(start, end) {
            left = _left;
            op = _op;
            right = _right;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            left.SetVariableContainer(container);
            right.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) {
            left.SetTypeHint(typeHint);
            right.SetTypeHint(typeHint);
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            IOperatorEmitter operatorEmitter =
                leftType.AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, rightType) ??
                rightType.AllowedPrefixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, leftType);

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.IncompatibleTypes,
                                       $"Cannot {op} a {leftType} with a {rightType}",
                                       Start,
                                       End
                                   ));
                return BuildinType.Unit;
            }

            return operatorEmitter.ResultType;
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            if (context.HasErrors) return;

            IOperatorEmitter leftEmitter = leftType.AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, rightType);
            IOperatorEmitter rightEmitter = rightType.AllowedPrefixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, leftType);

            if (leftEmitter == null && rightEmitter == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.IncompatibleTypes,
                                       $"Cannot {op} a {leftType} with a {rightType}",
                                       Start,
                                       End
                                   ));
                return;
            }

            left.EmitCode(context, false);
            if (rightEmitter != null) rightEmitter.OtherType.AssignFrom(context.ModuleContext, leftType).EmitConvert(context);
            right.EmitCode(context, false);
            if (leftEmitter != null) leftEmitter.OtherType.AssignFrom(context.ModuleContext, rightType).EmitConvert(context);

            if (context.HasErrors) return;

            if (leftEmitter != null) leftEmitter.EmitCode(context, this);
            else rightEmitter.EmitCode(context, this);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }
    }
}
