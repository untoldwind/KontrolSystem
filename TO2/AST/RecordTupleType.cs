using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class RecordTupleType : RecordType {
        private readonly SortedDictionary<string, TO2Type> itemTypes;
        private Type generatedType;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public RecordTupleType(IEnumerable<(string name, TO2Type type)> _itemTypes) : base(BuildinType.NO_OPERATORS) {
            itemTypes = new SortedDictionary<string, TO2Type>();
            foreach (var kv in _itemTypes) itemTypes.Add(kv.name, kv.type);
            allowedFields = itemTypes.Keys.Select((name, idx) => (name, new TupleFieldAccessFactory(this, itemTypes.Values.ToList(), idx) as IFieldAccessFactory)).ToDictionary(item => item.Item1, item => item.Item2);
        }

        public override SortedDictionary<string, TO2Type> ItemTypes => itemTypes;

        public override string Name => $"({String.Join(", ", itemTypes.Select(kv => $"{kv.Key} : {kv.Value}"))})";

        public override bool IsValid(ModuleContext context) => itemTypes.Count > 0 && itemTypes.Values.All(t => t.IsValid(context));

        public override RealizedType UnderlyingType(ModuleContext context) => new RecordTupleType(itemTypes.Select(kv => (kv.Key, kv.Value.UnderlyingType(context) as TO2Type)));

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = TupleType.DeriveTupleType(itemTypes.Values.Select(t => t.GeneratedType(context)).ToList()));

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            Type generatedType = GeneratedType(context);
            Type generatedOther = otherType.GeneratedType(context);
            RecordType otherRecordType = otherType as RecordType;

            return otherRecordType != null && generatedType != generatedOther ? new AssignRecordTuple(this, otherRecordType) : DefaultAssignEmitter.Instance;
        }

        internal override IOperatorEmitter CombineFrom(RecordType otherType) => new AssignRecordTuple(this, otherType);
    }

    internal class AssignRecordTuple : RecordTypeAssignEmitter<RecordTupleType> {
        internal AssignRecordTuple(RecordTupleType _targetType, RecordType _sourceType) : base(_targetType, _sourceType) { }

        protected override void EmitAssignToPtr(IBlockContext context, IBlockVariable tempSource) {
            Type type = targetType.GeneratedType(context.ModuleContext);

            int i = 0;
            foreach (var kv in targetType.ItemTypes) {
                if (i > 0 && i % 7 == 0) {
                    context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                    type = type.GetGenericArguments()[7];
                }
                IFieldAccessFactory sourceFieldFactory = sourceType.FindField(context.ModuleContext, kv.Key);
                if (sourceFieldFactory != null) {
                    IFieldAccessEmitter sourceField = sourceFieldFactory.Create(context.ModuleContext);

                    context.IL.Emit(OpCodes.Dup);
                    if (sourceField.RequiresPtr) tempSource.EmitLoadPtr(context);
                    else tempSource.EmitLoad(context);
                    sourceField.EmitLoad(context);
                    targetType.ItemTypes[kv.Key].AssignFrom(context.ModuleContext, sourceType.ItemTypes[kv.Key]).EmitConvert(context);
                    context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
                }
                i++;
            }
            context.IL.Emit(OpCodes.Pop);
        }
    }
}
