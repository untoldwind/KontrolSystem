using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class RangeCreate : Expression {
        public readonly Expression from;
        public readonly Expression to;
        public readonly bool inclusive;

        public RangeCreate(Expression from, Expression to, bool inclusive, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.from = from;
            this.to = to;
            this.inclusive = inclusive;

            from.SetTypeHint(_ => BuildinType.Int);
            this.to.SetTypeHint(_ => BuildinType.Int);
        }

        public override void SetVariableContainer(IVariableContainer container) {
            from.SetVariableContainer(container);
            to.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) {
        }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Range;

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            IBlockVariable tempVariable = context.MakeTempVariable(BuildinType.Range);
            EmitStore(context, tempVariable, dropResult);
        }

        public override void Prepare(IBlockContext context) {
            from.Prepare(context);
            to.Prepare(context);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            if (!BuildinType.Int.IsAssignableFrom(context.ModuleContext, from.ResultType(context)))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Range can only be created from int values",
                    from.Start,
                    from.End
                ));
            if (!BuildinType.Int.IsAssignableFrom(context.ModuleContext, to.ResultType(context)))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Range can only be created from int values",
                    to.Start,
                    to.End
                ));

            if (context.HasErrors) return;

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            from.EmitCode(context, false);
            context.IL.Emit(OpCodes.Stfld, typeof(Range).GetField("from"));
            to.EmitCode(context, false);
            if (inclusive) {
                context.IL.Emit(OpCodes.Ldc_I4_1);
                context.IL.Emit(OpCodes.Conv_I8);
                context.IL.Emit(OpCodes.Add);
            }

            context.IL.Emit(OpCodes.Stfld, typeof(Range).GetField("to"));

            if (!dropResult) variable.EmitLoad(context);
        }
    }
}
