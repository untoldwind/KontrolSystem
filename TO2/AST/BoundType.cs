using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class BoundType : RealizedType {
        public readonly string modulePrefix;
        public readonly string localName;
        public readonly string description;
        public readonly Type runtimeType;
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly OperatorCollection allowedSuffixOperators;
        public readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
        public readonly Dictionary<string, IFieldAccessFactory> allowedFields;

        public BoundType(string _modulePrefix, string _localName, string _description, Type _runtimeType,
                         OperatorCollection _allowedPrefixOperators,
                         OperatorCollection _allowedSuffixOperators,
                         IEnumerable<(string name, IMethodInvokeFactory invoker)> _allowedMethods,
                         IEnumerable<(string name, IFieldAccessFactory access)> _allowedFields) {
            modulePrefix = _modulePrefix;
            localName = _localName;
            description = _description;
            runtimeType = _runtimeType;
            allowedPrefixOperators = _allowedPrefixOperators;
            allowedSuffixOperators = _allowedSuffixOperators;
            allowedMethods = _allowedMethods.ToDictionary(m => m.name, m => m.invoker);
            allowedFields = _allowedFields.ToDictionary(m => m.name, m => m.access);
        }

        public override string Name => modulePrefix + "::" + localName;

        public override string Description => description;

        public override string LocalName => localName;

        public override bool IsValid(ModuleContext context) => !runtimeType.IsGenericType;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => runtimeType;

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override string[] GenericParameters => runtimeType.GetGenericArguments().Select(t => t.Name).ToArray();

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (runtimeType.IsGenericType) {
                Type[] arguments = runtimeType.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name)) throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();

                return new BoundType(modulePrefix, localName, description, runtimeType.MakeGenericType(arguments),
                                     allowedPrefixOperators, allowedSuffixOperators,
                                     allowedMethods.Select(m => (m.Key, m.Value.FillGenerics(context, typeArguments))),
                                     allowedFields.Select(f => (f.Key, f.Value.FillGenerics(context, typeArguments))));
            }
            return this;
        }
    }
}
