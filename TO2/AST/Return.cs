using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class ReturnEmpty : Expression {
        public ReturnEmpty(Position start = new Position(), Position end = new Position()) : base(start, end) {
        }

        public override void SetVariableContainer(IVariableContainer container) {
        }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (context.ExpectedReturn != BuildinType.Unit) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Expected a return value of type {context.ExpectedReturn}",
                    Start,
                    End
                ));
                return;
            }

            context.IL.Emit(OpCodes.Ldnull);
            if (context.IsAsync) {
                context.IL.EmitNew(OpCodes.Newobj,
                    context.MethodBuilder.ReturnType.GetConstructor(new Type[] {typeof(object)}));
            }

            context.IL.EmitReturn(context.MethodBuilder.ReturnType);
        }
    }

    public class ReturnValue : Expression {
        public readonly Expression returnValue;

        public ReturnValue(Expression returnValue, Position start = new Position(), Position end = new Position()) :
            base(start, end) {
            this.returnValue = returnValue;
            this.returnValue.SetTypeHint(context => context.ExpectedReturn.UnderlyingType(context.ModuleContext));
        }

        public override void SetVariableContainer(IVariableContainer container) =>
            returnValue.SetVariableContainer(container);

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type returnType = returnValue.ResultType(context);

            if (!context.ExpectedReturn.IsAssignableFrom(context.ModuleContext, returnType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Expected a return value of type {context.ExpectedReturn}, but got {returnType}",
                    Start,
                    End
                ));
                return;
            }

            returnValue.EmitCode(context, false);

            if (context.HasErrors) return;

            context.ExpectedReturn.AssignFrom(context.ModuleContext, returnType).EmitConvert(context);
            if (context.IsAsync) {
                context.IL.EmitNew(OpCodes.Newobj,
                    context.MethodBuilder.ReturnType.GetConstructor(new Type[]
                        {returnType.GeneratedType(context.ModuleContext)}));
            }

            context.IL.EmitReturn(context.MethodBuilder.ReturnType);
        }
    }
}
