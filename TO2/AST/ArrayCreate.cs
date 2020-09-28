using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class ArrayCreate : Expression {
        private readonly TO2Type elementType;
        private readonly List<Expression> elements;

        private TypeHint typeHint;

        public ArrayCreate(TO2Type elementType, List<Expression> elements, Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.elementType = elementType;
            this.elements = elements;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            foreach (Expression element in elements) element.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) {
            this.typeHint = typeHint;
            foreach (Expression element in elements)
                if (elementType != null)
                    element.SetTypeHint(context => elementType.UnderlyingType(context.ModuleContext));
                else
                    element.SetTypeHint(context => (this.typeHint?.Invoke(context) as ArrayType)?.elementType.UnderlyingType(context.ModuleContext));
        }

        public override TO2Type ResultType(IBlockContext context) {
            if (elementType != null) return new ArrayType(elementType);
            foreach (Expression element in elements) {
                TO2Type valueType = element.ResultType(context);
                if (valueType != BuildinType.Unit) return new ArrayType(valueType);
            }
            ArrayType arrayHint = typeHint?.Invoke(context) as ArrayType;

            return arrayHint ?? BuildinType.Unit;
        }

        public override void Prepare(IBlockContext context) {
            foreach (Expression element in elements) element.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            ArrayType arrayType = ResultType(context) as ArrayType;

            if (arrayType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of array. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            RealizedType elementType = arrayType.elementType.UnderlyingType(context.ModuleContext);

            context.IL.Emit(OpCodes.Ldc_I4, elements.Count);
            context.IL.Emit(OpCodes.Newarr, elementType.GeneratedType(context.ModuleContext));

            for (int i = 0; i < elements.Count; i++) {
                TO2Type valueType = elements[i].ResultType(context);
                if (!elementType.IsAssignableFrom(context.ModuleContext, valueType)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.InvalidType,
                                           $"Element {i} is of type {valueType}, expected {elementType}",
                                           elements[i].Start,
                                           elements[i].End
                                       ));
                }
            }

            if (context.HasErrors) return;

            for (int i = 0; i < elements.Count; i++) {
                context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(OpCodes.Ldc_I4, i);
                elements[i].EmitCode(context, false);
                if (elementType == BuildinType.Bool) context.IL.Emit(OpCodes.Stelem_I4);
                else if (elementType == BuildinType.Int) context.IL.Emit(OpCodes.Stelem_I8);
                else if (elementType == BuildinType.Float) context.IL.Emit(OpCodes.Stelem_R8);
                else context.IL.Emit(OpCodes.Stelem, elementType.GeneratedType(context.ModuleContext));
            }
        }
    }
}
