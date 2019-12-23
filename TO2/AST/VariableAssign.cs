using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class VariableAssign : Expression {
        public readonly string name;
        public readonly Operator op;
        public readonly Expression expression;
        private IVariableContainer variableContainer = null;

        public VariableAssign(string _name, Operator _op, Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            name = _name;
            op = _op;
            expression = _expression;
            expression.SetTypeHint(context => ResultType(context).UnderlyingType(context.ModuleContext));
        }

        public override void SetVariableContainer(IVariableContainer container) {
            expression.SetVariableContainer(container);
            variableContainer = container;
        }

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) => variableContainer?.FindVariable(context, name) ?? BuildinType.Unit;

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            IBlockVariable blockVariable = context.FindVariable(name);

            if (blockVariable == null)
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchVariable,
                                       $"No local variable '{name}'",
                                       Start,
                                       End
                                   ));
            else if (blockVariable.IsConst)
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchVariable,
                                       $"Local variable '{name}' is read-only (const)",
                                       Start,
                                       End
                                   ));

            if (context.HasErrors) return;

            TO2Type valueType = expression.ResultType(context);

            if (context.HasErrors) return;

            if (op == Operator.Assign) {
                EmitAssign(context, blockVariable, valueType, dropResult);
                return;
            }

            IOperatorEmitter operatorEmitter = blockVariable.Type.AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, valueType);

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                           StructuralError.ErrorType.IncompatibleTypes,
                           $"Cannot {op} a {blockVariable.Type} with a {valueType}",
                           Start,
                           End
                       ));
                return;
            }

            blockVariable.EmitLoad(context);
            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            operatorEmitter.EmitAssign(context, blockVariable, this);

            if (!dropResult) blockVariable.EmitLoad(context);
        }

        private void EmitAssign(IBlockContext context, IBlockVariable blockVariable, TO2Type valueType, bool dropResult) {
            if (!blockVariable.Type.IsAssignableFrom(context.ModuleContext, valueType))
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.IncompatibleTypes,
                                       $"Variable '{name}' is of type {blockVariable.Type} but is assigned to {valueType}",
                                       Start,
                                       End
                                   ));

            if (context.HasErrors) return;

            blockVariable.Type.AssignFrom(context.ModuleContext, valueType).EmitAssign(context, blockVariable, expression, dropResult);
        }
    }
}
