using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public struct ILCount {
        public int opCodes;
        public int stack;
    }

    public delegate RealizedType TypeHint(IBlockContext context);

    public abstract class Expression : Node, IBlockItem {
        public bool IsComment => false;

        public Expression(Position start, Position end) : base(start, end) { }

        public abstract void SetVariableContainer(IVariableContainer container);

        public abstract void SetTypeHint(TypeHint typeHint);

        public abstract TO2Type ResultType(IBlockContext context);

        public abstract void Prepare(IBlockContext context);

        public abstract void EmitCode(IBlockContext context, bool dropResult);

        public virtual void EmitPtr(IBlockContext context) {
            EmitCode(context, false);

            if (context.HasErrors) return;

            ILocalRef tempLocal = context.IL.TempLocal(ResultType(context).GeneratedType(context.ModuleContext));
            if (tempLocal.LocalIndex < 256) {
                context.IL.Emit(OpCodes.Stloc_S, tempLocal);
                context.IL.Emit(OpCodes.Ldloca_S, tempLocal);
            } else {
                context.IL.Emit(OpCodes.Stloc, tempLocal);
                context.IL.Emit(OpCodes.Ldloca, tempLocal);
            }
        }

        public virtual void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            EmitCode(context, false);

            if (context.HasErrors) return;

            if (!dropResult) context.IL.Emit(OpCodes.Dup);

            variable.EmitStore(context);
        }

        public ILCount GetILCount(IBlockContext context, bool dropResult) {
            IBlockContext countingContext = context.CloneCountingContext();

            this.EmitCode(countingContext, dropResult);

            return new ILCount {
                opCodes = countingContext.IL.ILSize,
                stack = countingContext.IL.StackCount
            };
        }
    }

    public class Bracket : Expression {
        public readonly Expression expression;

        public Bracket(Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) => expression = _expression;

        public override void SetVariableContainer(IVariableContainer container) => expression.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint typeHint) => expression.SetTypeHint(typeHint);

        public override TO2Type ResultType(IBlockContext context) => expression.ResultType(context);

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) => expression.EmitCode(context, dropResult);
    }
}
