using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class TypeReference : TO2Type {
        private string moduleName;
        private string name;
        private List<TO2Type> typeArguments;

        public TypeReference(List<string> namePath, List<TO2Type> _typeArguments) {
            if (namePath.Count > 1) {
                moduleName = String.Join("::", namePath.Take(namePath.Count - 1));
                name = namePath.Last();
            } else {
                moduleName = null;
                name = namePath.Last();
            }
            typeArguments = _typeArguments;
        }

        public TypeReference(string _moduleName, string _name) {
            moduleName = _moduleName;
            name = _name;
        }

        public override string Name {
            get {
                StringBuilder builder = new StringBuilder();

                if (moduleName != null) {
                    builder.Append(moduleName);
                    builder.Append("::");
                }
                builder.Append(name);
                if (typeArguments.Count > 0) {
                    builder.Append("<");
                    builder.Append(String.Join(",", typeArguments.Select(t => t.Name)));
                    builder.Append(">");
                }
                return builder.ToString();
            }
        }

        public override bool IsValid(ModuleContext context) => ReferencedType(context) != null;

        public override RealizedType UnderlyingType(ModuleContext context) => ReferencedType(context);

        public override Type GeneratedType(ModuleContext context) => ReferencedType(context).GeneratedType(context);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => ReferencedType(context).AllowedPrefixOperators(context);

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => ReferencedType(context).AllowedSuffixOperators(context);

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) => ReferencedType(context).FindMethod(context, methodName);

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) => ReferencedType(context).FindField(context, fieldName);

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => ReferencedType(context).AllowedIndexAccess(context, indexSpec);

        public override IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => ReferencedType(context).ForInSource(context, typeHint);

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => ReferencedType(context).IsAssignableFrom(context, otherType);

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => ReferencedType(context).AssignFrom(context, otherType);

        private RealizedType ReferencedType(ModuleContext context) {
            RealizedType realizedType = moduleName != null ? context.FindModule(moduleName)?.FindType(name) : context.mappedTypes.Get(name)?.UnderlyingType(context);
            if (realizedType == null) {
                throw new CompilationErrorException(new List<StructuralError> {
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Unable to lookup type {Name}",
                        new Parsing.Position(),
                        new Parsing.Position()
                    )
                });
            }
            string[] typeParamaterNames = realizedType.GenericParameters;
            if (typeParamaterNames.Length != typeArguments.Count) {
                throw new CompilationErrorException(new List<StructuralError> {
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Type {realizedType.Name} expects {typeParamaterNames.Length} type parameters, only {typeArguments.Count} where given",
                        new Parsing.Position(),
                        new Parsing.Position()
                    )
                });
            }
            Dictionary<string, RealizedType> namedTypeArguments = new Dictionary<string, RealizedType>();
            for (int i = 0; i < typeArguments.Count; i++) {
                namedTypeArguments.Add(typeParamaterNames[i], typeArguments[i].UnderlyingType(context));
            }

            return realizedType.FillGenerics(context, namedTypeArguments);
        }
    }
}
