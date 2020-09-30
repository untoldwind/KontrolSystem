using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class ForIn : Expression, IVariableContainer {
        public readonly string variableName;
        public readonly TO2Type variableType;
        public readonly Expression sourceExpression;
        public readonly Expression loopExpression;

        private IVariableContainer parentContainer;

        public ForIn(string variableName, TO2Type variableType, Expression sourceExpression, Expression loopExpression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.variableName = variableName;
            this.variableType = variableType;
            this.sourceExpression = sourceExpression;
            if (this.variableType != null)
                this.sourceExpression.SetTypeHint(context =>
                    new ArrayType(this.variableType.UnderlyingType(context.ModuleContext)));
            this.loopExpression = loopExpression;
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) {
            if (name != variableName) return null;
            return variableType ?? sourceExpression.ResultType(context)?.ForInSource(context.ModuleContext, null)
                .ElementType;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            parentContainer = container;
            sourceExpression.SetVariableContainer(this);
            loopExpression.SetVariableContainer(this);
        }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            RealizedType sourceType = sourceExpression.ResultType(context).UnderlyingType(context.ModuleContext);
            IForInSource source = sourceType.ForInSource(context.ModuleContext, variableType);

            if (source == null)
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{sourceType} cannot be use as for ... in source",
                        Start,
                        End
                    )
                );
            if (context.FindVariable(variableName) != null)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.DublicateVariableName,
                    $"Variable '{variableName}' already declared in this scope",
                    Start,
                    End
                ));
            if (variableType != null && !variableType.IsAssignableFrom(context.ModuleContext, source.ElementType))
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{sourceType} has elements of type {source.ElementType}, expected {variableType}",
                        Start,
                        End
                    )
                );

            if (context.HasErrors) return;

            ILCount loopSize = EstimateLoop(context, source);
            LabelRef start = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef end = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef loop = context.IL.DefineLabel(loopSize.opCodes < 114);

            IBlockContext loopContext = context.CreateLoopContext(start, end);
            IBlockVariable loopVariable = loopContext.DeclaredVariable(variableName, true, source.ElementType);

            sourceExpression.EmitCode(context, false);

            if (context.HasErrors) return;

            source.EmitInitialize(loopContext);
            loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);

            loopContext.IL.MarkLabel(loop);

            // Timeout check
            context.IL.EmitCall(OpCodes.Call, typeof(KontrolSystem.TO2.Runtime.ContextHolder).GetMethod("CheckTimeout"),
                0);

            source.EmitNext(loopContext);
            loopVariable.EmitStore(loopContext);
            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(start);
            source.EmitCheckDone(loopContext, loop);
            loopContext.IL.MarkLabel(end);
            if (!dropResult) context.IL.Emit(OpCodes.Ldnull);
        }

        private ILCount EstimateLoop(IBlockContext context, IForInSource source) {
            IBlockContext prepContext = context.CloneCountingContext();

            sourceExpression.EmitCode(prepContext, false);
            source.EmitInitialize(prepContext);

            IBlockContext countingContext = prepContext.CloneCountingContext()
                .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            IBlockVariable loopVariable = countingContext.DeclaredVariable(variableName, true, source.ElementType);
            LabelRef loop = countingContext.IL.DefineLabel(false);

            source.EmitNext(countingContext);
            loopVariable.EmitStore(countingContext);
            loopExpression.EmitCode(countingContext, true);
            source.EmitCheckDone(countingContext, loop);

            return new ILCount {
                opCodes = countingContext.IL.ILSize,
                stack = countingContext.IL.StackCount
            };
        }
    }
}
