using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class ArrayBuilderCreate : Expression {
        public readonly TO2Type elementType;
        public readonly Expression expression;

        private TypeHint typeHint;

        public ArrayBuilderCreate(TO2Type _elementType, Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            elementType = _elementType;
            expression = _expression;
        }

        public override void SetVariableContainer(IVariableContainer container) => expression.SetVariableContainer(container);

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            expression.SetTypeHint(context => (typeHint?.Invoke(context) as OptionType)?.elementType.UnderlyingType(context.ModuleContext));
        }

        public override TO2Type ResultType(IBlockContext context) {
            if (elementType != null) return new ArrayBuilderType(elementType);

            return typeHint?.Invoke(context) as ArrayBuilderType;
        }

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type resultType = expression.ResultType(context);
            ArrayBuilderType cellType = ResultType(context) as ArrayBuilderType;

            if (cellType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of array builder element. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            if (!BuildinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Argument of ArrayBuilder has to be an int",
                                       Start,
                                       End
                                   ));
                return;
            }

            Type generatedType = cellType.GeneratedType(context.ModuleContext);
            ConstructorInfo constructor = generatedType.GetConstructor(new Type[] { typeof(long) });

            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            cellType.elementType.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

            context.IL.EmitNew(OpCodes.Newobj, constructor, 1);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }
    }
}
