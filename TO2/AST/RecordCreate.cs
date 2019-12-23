using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class RecordCreate : Expression {
        public readonly Dictionary<string, Expression> items;
        private RecordType resultType;

        private TypeHint typeHint;

        public RecordCreate(IEnumerable<(string, Expression)> _items, Position start, Position end) : base(start, end) {
            items = _items.ToDictionary(kv => kv.Item1, kv => kv.Item2);
        }

        public override void SetVariableContainer(IVariableContainer container) {
            foreach (var kv in items) kv.Value.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
            foreach (var kv in items) {
                string itemName = kv.Key;
                kv.Value.SetTypeHint(context => {
                    SortedDictionary<string, TO2Type> itemTypes = (typeHint?.Invoke(context) as RecordType)?.ItemTypes;

                    return itemTypes.Get(itemName)?.UnderlyingType(context.ModuleContext);
                });
            }
        }

        public override TO2Type ResultType(IBlockContext context) => DeriveType(context);

        public override void Prepare(IBlockContext context) {
            foreach (Expression item in items.Values) item.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            RecordType recordHint = ResultType(context) as RecordType;

            IBlockVariable tempVariable = context.MakeTempVariable(recordHint ?? DeriveType(context));
            EmitStore(context, tempVariable, dropResult);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            RecordType recordType = ResultType(context) as RecordType;
            if (recordType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"{variable.Type} is not a record",
                                       Start,
                                       End
                                   ));
                return;
            } else {
                foreach (var kv in items) {
                    if (!recordType.ItemTypes.ContainsKey(kv.Key))
                        context.AddError(new StructuralError(
                                               StructuralError.ErrorType.IncompatibleTypes,
                                               $"{recordType} does not have a field {kv.Key}",
                                               Start,
                                               End
                                           ));
                    else {
                        TO2Type valueType = kv.Value.ResultType(context);
                        if (!recordType.ItemTypes[kv.Key].IsAssignableFrom(context.ModuleContext, valueType)) {
                            context.AddError(new StructuralError(
                                                StructuralError.ErrorType.IncompatibleTypes,
                                                $"Expected item {kv.Key} of {recordType} to be a {recordType.ItemTypes[kv.Key]}, found {valueType}",
                                                Start,
                                                End
                                            ));
                        }
                    }
                }
                foreach (string name in recordType.ItemTypes.Keys)
                    if (!items.ContainsKey(name))
                        context.AddError(new StructuralError(
                                               StructuralError.ErrorType.IncompatibleTypes,
                                               $"Missing {name} for of {recordType}",
                                               Start,
                                               End
                                           ));
            }

            if (context.HasErrors) return;

            foreach (Expression item in items.Values) {
                item.Prepare(context);
            }

            Type type = recordType.GeneratedType(context.ModuleContext);

            variable.EmitLoadPtr(context);
            // Note: Potentially overoptimized: Since all fields are set, initialization should not be necessary
            //            context.IL.Emit(OpCodes.Dup);
            //            context.IL.Emit(OpCodes.Initobj, type, 1, 0);
            int i = 0;
            foreach (var kv in recordType.ItemTypes) {
                if (i > 0 && i % 7 == 0) {
                    context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                    type = type.GetGenericArguments()[7];
                    //                    context.IL.Emit(OpCodes.Dup);
                    //                    context.IL.Emit(OpCodes.Initobj, type, 1, 0);
                }
                if (i < items.Count - 1) context.IL.Emit(OpCodes.Dup);
                items[kv.Key].EmitCode(context, false);
                context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
                i++;
            }

            if (context.HasErrors) return;

            if (!dropResult) variable.EmitLoad(context);
        }

        private RecordType DeriveType(IBlockContext context) {
            if (resultType == null) resultType = new RecordTupleType(items.Select(item => (item.Key, item.Value.ResultType(context))));
            return resultType;
        }
    }
}
