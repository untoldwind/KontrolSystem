using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

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
        public readonly IEnumerable<RealizedType> typeParameters;

        public BoundType(string _modulePrefix, string _localName, string _description, Type _runtimeType,
                         OperatorCollection _allowedPrefixOperators,
                         OperatorCollection _allowedSuffixOperators,
                         IEnumerable<(string name, IMethodInvokeFactory invoker)> _allowedMethods,
                         IEnumerable<(string name, IFieldAccessFactory access)> _allowedFields,
                         IEnumerable<RealizedType> _typeParameters = null) {
            modulePrefix = _modulePrefix;
            localName = _localName;
            description = _description;
            runtimeType = _runtimeType;
            allowedPrefixOperators = _allowedPrefixOperators;
            allowedSuffixOperators = _allowedSuffixOperators;
            allowedMethods = _allowedMethods.ToDictionary(m => m.name, m => m.invoker);
            allowedFields = _allowedFields.ToDictionary(m => m.name, m => m.access);
            typeParameters = _typeParameters ?? _runtimeType.GetGenericArguments().Select(t => new GenericParameter(t.Name));
        }

        public override string Name {
            get {
                StringBuilder builder = new StringBuilder();

                if (modulePrefix != null) {
                    builder.Append(modulePrefix);
                    builder.Append("::");
                }
                builder.Append(localName);
                if (typeParameters.Any()) {
                    builder.Append("<");
                    builder.Append(String.Join(",", typeParameters.Select(t => t.Name)));
                    builder.Append(">");
                }
                return builder.ToString();
            }
        }

        public override string Description => description;

        public override string LocalName => localName;

        public override bool IsValid(ModuleContext context) => !runtimeType.IsGenericTypeDefinition;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => runtimeType;

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override string[] GenericParameters => typeParameters.SelectMany(t => (t as GenericParameter)?.Name.Yield() ?? Enumerable.Empty<string>()).ToArray();

        public override RealizedType FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (runtimeType.IsGenericType) {
                IEnumerable<RealizedType> filled = typeParameters.Select(t => t.FillGenerics(context, typeArguments));

                if (filled.Any(t => t is GenericParameter)) {
                    return new BoundType(modulePrefix, localName, description, runtimeType,
                                        allowedPrefixOperators,
                                        allowedSuffixOperators,
                                        allowedMethods.Select(m => (m.Key, m.Value)),
                                        allowedFields.Select(f => (f.Key, f.Value)),
                                        filled);
                }
                Type[] arguments = filled.Select(t => t.GeneratedType(context)).ToArray();
                Dictionary<string, RealizedType> orignalTypeArguments = runtimeType.GetGenericArguments().Zip(filled, (o, t) => (o.Name, t)).ToDictionary(i => i.Item1, i => i.Item2);

                return new BoundType(modulePrefix, localName, description, runtimeType.MakeGenericType(arguments),
                                     allowedPrefixOperators.FillGenerics(context, orignalTypeArguments),
                                     allowedSuffixOperators.FillGenerics(context, orignalTypeArguments),
                                     allowedMethods.Select(m => (m.Key, m.Value.FillGenerics(context, orignalTypeArguments))),
                                     allowedFields.Select(f => (f.Key, f.Value.FillGenerics(context, orignalTypeArguments))),
                                     filled);
            }
            return this;
        }

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context, RealizedType concreteType) {
            if (!runtimeType.IsGenericType) return Enumerable.Empty<(string name, RealizedType type)>();

            BoundType otherBoundType = concreteType as BoundType;
            if (otherBoundType == null || otherBoundType.runtimeType.GetGenericTypeDefinition() != runtimeType) return Enumerable.Empty<(string name, RealizedType type)>();

            return typeParameters.Zip(otherBoundType.typeParameters, (t, o) => t.InferGenericArgument(context, o)).SelectMany(t => t);
        }
    }
}
