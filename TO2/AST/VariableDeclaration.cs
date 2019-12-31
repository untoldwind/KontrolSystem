using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IVariableContainer {
        IVariableContainer ParentContainer {
            get;
        }

        TO2Type FindVariableLocal(IBlockContext context, string name);
    }

    public static class VariableContainerExtensions {
        public static TO2Type FindVariable(this IVariableContainer current, IBlockContext context, string name) {
            while (current != null) {
                TO2Type variableType = current.FindVariableLocal(context, name);

                if (variableType != null) return variableType;
                current = current.ParentContainer;
            }
            return null;
        }
    }

    public class DeclarationParameter {
        public readonly string target;
        public readonly string source;
        public readonly TO2Type type;

        public DeclarationParameter() {
            target = null;
            source = null;
            type = null;
        }

        public DeclarationParameter(string _target, string _source) {
            target = _target;
            source = _source;
            type = null;
        }

        public DeclarationParameter(string _target, string _source, TO2Type _type) {
            target = _target;
            source = _source;
            type = _type;
        }

        public bool IsPlaceholder => target == null;

        public bool IsInferred => type == null;
    }

    public class VariableDeclaration : Node, IBlockItem, IVariableRef {
        public readonly DeclarationParameter declaration;
        public readonly bool isConst;
        public readonly Expression expression;
        private IVariableContainer variableContainer;
        private bool lookingUp = false;

        public VariableDeclaration(DeclarationParameter _declaration, bool _isConst, Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            declaration = _declaration;
            isConst = _isConst;
            expression = _expression;
            expression.SetTypeHint(context => declaration.type?.UnderlyingType(context.ModuleContext));
        }

        public bool IsComment => false;

        public void SetVariableContainer(IVariableContainer container) {
            expression.SetVariableContainer(container);
            variableContainer = container;
        }

        public void SetTypeHint(TypeHint typeHint) { }

        public TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public string Name => declaration.target;

        public TO2Type VariableType(IBlockContext context) {
            if (lookingUp) return null;
            lookingUp = true; // Somewhat ugly workaround if there is a cycle in inferred variables that should produce a correct error message
            TO2Type type = declaration.IsInferred ? expression.ResultType(context) : declaration.type;
            lookingUp = false;
            return type;
        }

        public void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type valueType = expression.ResultType(context);
            TO2Type variableType = declaration.IsInferred ? valueType : declaration.type;

            if (context.HasErrors) return;

            if (context.FindVariable(declaration.target) != null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.DublicateVariableName,
                                       $"Variable '{declaration.target}' already declared in this scope",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (!variableType.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.IncompatibleTypes,
                                       $"Variable '{declaration.target}' is of type {variableType} but is initialized with {valueType}",
                                       Start,
                                       End
                                   ));
                return;
            }

            IBlockVariable variable = context.DeclaredVariable(declaration.target, isConst, variableType.UnderlyingType(context.ModuleContext));

            variable.Type.AssignFrom(context.ModuleContext, valueType).EmitAssign(context, variable, expression, true);
        }
    }
}
