using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IIndexAccessEmitter {
        TO2Type TargetType { get; }

        bool RequiresPtr { get; }

        void EmitLoad(IBlockContext context);
    }

    public class InlineArrayIndexAccessEmitter : IIndexAccessEmitter {
        private readonly RealizedType targetType;
        private readonly RealizedType indexType;
        private readonly Expression indexExpression;

        public InlineArrayIndexAccessEmitter(RealizedType targetType, RealizedType indexType, Expression indexExpression) {
            this.targetType = targetType;
            this.indexType = indexType;
            this.indexExpression = indexExpression;
            this.indexExpression.SetTypeHint(_ => BuildinType.Int);
        }

        public TO2Type TargetType => targetType;

        public bool RequiresPtr => false;

        public void EmitLoad(IBlockContext context) {
            if (!indexType.IsAssignableFrom(context.ModuleContext, indexExpression.ResultType(context))) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Index has to be of type {indexType}",
                                       indexExpression.Start,
                                       indexExpression.End
                                   ));
                return;
            }
            indexExpression.EmitCode(context, false);

            context.IL.Emit(OpCodes.Conv_I4);
            if (targetType == BuildinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
            else if (targetType == BuildinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
            else if (targetType == BuildinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
            else context.IL.Emit(OpCodes.Ldelem, targetType.GeneratedType(context.ModuleContext));
        }
    }
}
