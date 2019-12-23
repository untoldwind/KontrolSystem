using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class UnaryPrefix : Expression {
        public readonly Operator op;
        public readonly Expression right;

        public UnaryPrefix(Operator _op, Expression _right, Position start = new Position(), Position end = new Position()) : base(start, end) {
            op = _op;
            right = _right;
        }

        public override void SetVariableContainer(IVariableContainer container) => right.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint typeHint) => right.SetTypeHint(typeHint);

        public override TO2Type ResultType(IBlockContext context) => right.ResultType(context).AllowedPrefixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuildinType.Unit)?.ResultType ?? BuildinType.Unit;

        public override void Prepare(IBlockContext context) => right.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type rightType = right.ResultType(context);
            IOperatorEmitter operatorEmitter = rightType.AllowedPrefixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuildinType.Unit);

            if (context.HasErrors) return;

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidOperator,
                                       $"Prefix {op} on a {rightType} is undefined",
                                       Start,
                                       End
                                   ));
                return;
            }

            right.EmitCode(context, false);

            if (context.HasErrors) return;

            operatorEmitter.EmitCode(context, this);

            if (dropResult && operatorEmitter.ResultType != BuildinType.Unit) context.IL.Emit(OpCodes.Pop);
        }
    }
}
