using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class IndexAssign : Expression {
        private readonly Expression target;
        private readonly IndexSpec indexSpec;
        private readonly Operator op;
        private readonly Expression expression;

        public IndexAssign(Expression target, IndexSpec indexSpec, Operator op, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.target = target;
            this.indexSpec = indexSpec;
            this.op = op;
            this.expression = expression;
        }

        public override IVariableContainer VariableContainer {
            set {
                target.VariableContainer = value;
                indexSpec.VariableContainer = value;
                expression.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) =>
            target.ResultType(context)?.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ??
            BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
            indexSpec.start.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            throw new System.NotImplementedException();
        }
    }
}
