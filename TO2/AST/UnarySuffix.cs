using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class UnarySuffix : Expression {
        public readonly Expression left;
        public readonly Operator op;

        public UnarySuffix(Expression left, Operator op, Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.left = left;
            this.op = op;
        }

        public override void SetVariableContainer(IVariableContainer container) => left.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint typeHint) => left.SetTypeHint(typeHint);

        public override TO2Type ResultType(IBlockContext context) => left.ResultType(context).AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuildinType.Unit)?.ResultType ?? BuildinType.Unit;

        public override void Prepare(IBlockContext context) => left.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type leftType = left.ResultType(context);
            IOperatorEmitter operatorEmitter = leftType.AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuildinType.Unit);

            if (context.HasErrors) return;

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidOperator,
                                       $"Suffix {op} on a {leftType} is undefined",
                                       Start,
                                       End
                                   ));
                return;
            }

            left.EmitCode(context, false);

            if (context.HasErrors) return;

            operatorEmitter.EmitCode(context, this);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }
    }
}
