using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class TupleCreate : Expression {
        public readonly List<Expression> items;
        private TupleType resultType;

        private TypeHint typeHint;

        public TupleCreate(List<Expression> _items, Position start, Position end) : base(start, end) {
            items = _items;
        }

        public override void SetVariableContainer(IVariableContainer container) {
            foreach (Expression item in items) item.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            for (int j = 0; j < items.Count; j++) {
                int i = j; // Copy for lambda
                items[i].SetTypeHint(context => {
                    List<RealizedType> itemTypes = (typeHint?.Invoke(context) as TupleType)?.itemTypes.Select(t => t.UnderlyingType(context.ModuleContext)).ToList();

                    return itemTypes != null && i < itemTypes.Count ? itemTypes[i] : null;
                });
            }
        }

        public override TO2Type ResultType(IBlockContext context) => DeriveType(context);

        public override void Prepare(IBlockContext context) {
            foreach (Expression item in items) {
                item.Prepare(context);
            }
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            TupleType tupleHint = ResultType(context) as TupleType;

            IBlockVariable tempVariable = context.MakeTempVariable(tupleHint ?? DeriveType(context));
            EmitStore(context, tempVariable, dropResult);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            TupleType tupleType = variable.Type as TupleType;
            if (tupleType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"{variable.Type} is not a tuple",
                                       Start,
                                       End
                                   ));
                return;
            } else {
                if (items.Count != tupleType.itemTypes.Count) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.InvalidType,
                                           $"Expected tuple of {tupleType.itemTypes.Count} items, found {items.Count} items",
                                           Start,
                                           End
                                       ));
                }
                for (int i = 0; i < items.Count; i++) {
                    TO2Type valueType = items[i].ResultType(context);
                    if (!tupleType.itemTypes[i].IsAssignableFrom(context.ModuleContext, valueType)) {
                        context.AddError(new StructuralError(
                                               StructuralError.ErrorType.InvalidType,
                                               $"Expected item {i} of {tupleType} to be a {tupleType.itemTypes[i]}, found {valueType}",
                                               Start,
                                               End
                                           ));
                    }
                }
            }

            if (context.HasErrors) return;

            foreach (Expression item in items) {
                item.Prepare(context);
            }

            Type type = tupleType.GeneratedType(context.ModuleContext);

            variable.EmitLoadPtr(context);
            // Note: Potentially overoptimized: Since all fields are set, initialization should not be necessary
            //            context.IL.Emit(OpCodes.Dup);
            //            context.IL.Emit(OpCodes.Initobj, type, 1, 0);

            for (int i = 0; i < items.Count; i++) {
                if (i > 0 && i % 7 == 0) {
                    context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                    type = type.GetGenericArguments()[7];
                    //                    context.IL.Emit(OpCodes.Dup);
                    //                    context.IL.Emit(OpCodes.Initobj, type, 1, 0);
                }
                if (i < items.Count - 1) context.IL.Emit(OpCodes.Dup);
                items[i].EmitCode(context, false);
                tupleType.itemTypes[i].AssignFrom(context.ModuleContext, items[i].ResultType(context)).EmitConvert(context);
                context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
            }

            if (context.HasErrors) return;

            if (!dropResult) variable.EmitLoad(context);
        }

        private TupleType DeriveType(IBlockContext context) {
            if (resultType == null) {
                resultType = new TupleType(items.Select(item => item.ResultType(context)).ToList());
            }
            return resultType;
        }
    }
}
