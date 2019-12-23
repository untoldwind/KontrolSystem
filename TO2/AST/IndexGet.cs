using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class IndexGet : Expression {
        public readonly Expression target;
        public readonly IndexSpec indexSpec;

        public IndexGet(Expression _target, IndexSpec _indexSpec, Position start = new Position(), Position end = new Position()) : base(start, end) {
            target = _target;
            indexSpec = _indexSpec;
        }

        public override void SetVariableContainer(IVariableContainer container) => target.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            return targetType.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ?? BuildinType.Unit;
        }

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
            indexSpec.start.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type targetType = target.ResultType(context);
            IIndexAccessEmitter indexAccess = targetType.AllowedIndexAccess(context.ModuleContext, indexSpec);

            if (indexAccess == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoIndexAccess,
                                       $"Type '{targetType.Name}' does not support access by index",
                                       Start,
                                       End
                                   ));
                return;
            }

            if (!dropResult) {
                if (indexAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                indexAccess.EmitLoad(context);
            }
        }
    }
}
