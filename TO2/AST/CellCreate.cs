using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class CellCreate : Expression {
        public readonly Expression expression;

        private TypeHint typeHint;

        public CellCreate(Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            expression = _expression;
        }

        public override void SetVariableContainer(IVariableContainer container) => expression.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            expression.SetTypeHint(context => (typeHint?.Invoke(context) as OptionType)?.elementType.UnderlyingType(context.ModuleContext));
        }

        public override TO2Type ResultType(IBlockContext context) {
            CellType cellHint = typeHint?.Invoke(context) as CellType;

            return cellHint ?? new CellType(expression.ResultType(context));
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type resultType = expression.ResultType(context);
            CellType cellType = ResultType(context) as CellType;

            if (cellType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of cell. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            if (!cellType.elementType.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Cell of type {cellType} cannot be create from a {resultType}.",
                                       Start,
                                       End
                                   ));
                return;
            }

            Type generatedType = cellType.GeneratedType(context.ModuleContext);
            ConstructorInfo constructor = generatedType.GetConstructor(new Type[] { cellType.elementType.GeneratedType(context.ModuleContext) });

            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            cellType.elementType.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

            context.IL.EmitNew(OpCodes.Newobj, constructor, 1);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }
    }
}
