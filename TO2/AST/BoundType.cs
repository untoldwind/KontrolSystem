using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class BoundType : RealizedType {
        public readonly string modulePrefix;
        public readonly string localName;
        public readonly string description;
        public readonly Type runtimeType;
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly OperatorCollection allowedPostfixOperators;
        public readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
        public readonly Dictionary<string, IFieldAccessFactory> allowedFields;

        public BoundType(string _modulePrefix, string _localName, string _description, Type _runtimeType,
                        OperatorCollection _allowedPrefixOperators,
                        OperatorCollection _allowedPostfixOperators,
                         Dictionary<string, IMethodInvokeFactory> _allowedMethods,
                         Dictionary<string, IFieldAccessFactory> _allowedFields) {
            modulePrefix = _modulePrefix;
            localName = _localName;
            description = _description;
            runtimeType = _runtimeType;
            allowedPrefixOperators = _allowedPrefixOperators;
            allowedPostfixOperators = _allowedPostfixOperators;
            allowedMethods = _allowedMethods;
            allowedFields = _allowedFields;
        }

        public override string Name => modulePrefix + "::" + localName;

        public override string Description => description;

        public override string LocalName => localName;

        public override bool IsValid(ModuleContext context) => true;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => runtimeType;

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedPostfixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null; // TODO: Actually this might be allowed
    }
}
