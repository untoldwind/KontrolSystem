using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class FunctionType : RealizedType {
        public readonly bool isAsync;
        public readonly List<TO2Type> parameterTypes;
        public readonly TO2Type returnType;
        private Type generatedType;

        public FunctionType(bool _isAsync, List<TO2Type> _parameterTypes, TO2Type _returnType) {
            isAsync = _isAsync;
            parameterTypes = _parameterTypes;
            returnType = _returnType;
        }

        public override string Name => $"fn({String.Join(", ", parameterTypes)}) -> {returnType}";

        public override bool IsValid(ModuleContext context) => returnType.IsValid(context) && parameterTypes.All(t => t.IsValid(context));

        public override RealizedType UnderlyingType(ModuleContext context) => new FunctionType(isAsync, parameterTypes.Select(p => p.UnderlyingType(context) as TO2Type).ToList(), returnType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) {
            if (generatedType == null) {
                if (returnType == BuildinType.Unit) {
                    if (parameterTypes.Count == 0)
                        generatedType = typeof(Action);
                    else
                        generatedType = Type.GetType($"System.Action`{parameterTypes.Count}").MakeGenericType(parameterTypes.Select(p => p.GeneratedType(context)).ToArray());
                } else {
                    generatedType = Type.GetType($"System.Func`{parameterTypes.Count + 1}").MakeGenericType(parameterTypes.Concat(returnType.Yield()).Select(p => p.GeneratedType(context)).ToArray());
                }
            }
            return generatedType;
        }

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => BuildinType.NO_METHODS;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => BuildinType.NO_FIELDS;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;
    }
}
