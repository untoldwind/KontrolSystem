using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class TupleType : RealizedType {
        public readonly List<TO2Type> itemTypes;
        private Type generatedType;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public TupleType(List<TO2Type> _itemTypes) {
            itemTypes = _itemTypes;
            allowedFields = itemTypes.Select((_, idx) => ($"_{idx + 1}", new TupleFieldAccessFactory(this, itemTypes, idx) as IFieldAccessFactory)).ToDictionary(item => item.Item1, item => item.Item2);
        }

        public override string Name => $"({String.Join(", ", itemTypes)})";

        public override bool IsValid(ModuleContext context) => itemTypes.Count > 0 && itemTypes.All(t => t.IsValid(context));

        public override RealizedType UnderlyingType(ModuleContext context) => new TupleType(itemTypes.Select(p => p.UnderlyingType(context) as TO2Type).ToList());

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = DeriveTupleType(itemTypes.Select(t => t.GeneratedType(context)).ToList()));

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            if (!(otherType.UnderlyingType(context) is TupleType)) return false;
            return GeneratedType(context).IsAssignableFrom(otherType.GeneratedType(context));
        }

        internal static Type DeriveTupleType(List<Type> itemTypes) {
            if (itemTypes.Count > 7) {
                Type rest = DeriveTupleType(itemTypes.Skip(7).ToList());
                return Type.GetType("System.ValueTuple`8").MakeGenericType(itemTypes.Take(7).Concat(rest.Yield()).ToArray());
            } else {
                return Type.GetType($"System.ValueTuple`{itemTypes.Count}").MakeGenericType(itemTypes.ToArray());
            }
        }
    }

    internal class TupleFieldAccessFactory : IFieldAccessFactory {
        private RealizedType tupleType;
        private List<TO2Type> itemTypes;
        private int index;

        internal TupleFieldAccessFactory(RealizedType _tupleType, List<TO2Type> _itemTypes, int _index) {
            tupleType = _tupleType;
            itemTypes = _itemTypes;
            index = _index;
        }

        public TO2Type DeclaredType => itemTypes[index];

        public string Description => itemTypes[index].Description;

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = tupleType.GeneratedType(context);
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            Type currentType = generateType;
            int currentIdx = index;

            while (currentIdx >= 7) {
                currentIdx -= 7;
                FieldInfo rest = currentType.GetField("Rest");
                fieldInfos.Add(rest);
                currentType = rest.FieldType;
            }
            fieldInfos.Add(currentType.GetField($"Item{currentIdx + 1}"));

            return new BoundFieldAccessEmitter(itemTypes[index].UnderlyingType(context), generateType, fieldInfos);
        }

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }
}
