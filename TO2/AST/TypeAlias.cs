using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using System;

namespace KontrolSystem.TO2.AST {
    public class TypeAlias : Node, IModuleItem {
        public readonly bool exported;
        public readonly string name;
        public readonly string description;
        public readonly TO2Type type;

        public TypeAlias(bool exported, string name, string description, TO2Type type, Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.exported = exported;
            this.name = name;
            this.description = description;
            this.type = type;
        }

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) {
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
            if (exported) context.exportedTypes.Add((name, new TypeAliasDelegate(context, type, description, this)));
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
            if (context.mappedTypes.ContainsKey(name))
                return new StructuralError(
                    StructuralError.ErrorType.DuplicateTypeName,
                    $"Type with name {name} already defined",
                    Start,
                    End
                ).Yield();
            context.mappedTypes.Add(name, type);
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
            return Enumerable.Empty<StructuralError>();
        }
    }

    public class TypeAliasDelegate : TO2Type {
        private readonly ModuleContext declaredModule;
        private TO2Type aliasedType;
        private bool lookingUp;
        private Node target;
        private string description;

        internal TypeAliasDelegate(ModuleContext declaredModule, TO2Type aliasedType, string description, Node target) {
            this.declaredModule = declaredModule;
            this.aliasedType = aliasedType;
            lookingUp = false;
            this.description = description;
            this.target = target;
        }

        public override string Name => aliasedType.Name;

        public override string Description => description;

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) => aliasedType.FindField(declaredModule, fieldName);

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => aliasedType.AllowedIndexAccess(declaredModule, indexSpec);

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) => aliasedType.FindMethod(declaredModule, methodName);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => aliasedType.AllowedPrefixOperators(declaredModule);

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => aliasedType.AllowedSuffixOperators(declaredModule);

        public override Type GeneratedType(ModuleContext context) => aliasedType.GeneratedType(declaredModule);

        public override RealizedType UnderlyingType(ModuleContext context) {
            if (lookingUp) throw new CompilationErrorException(new List<StructuralError> {
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Cyclic dependency to {aliasedType.Name}",
                    target.Start,
                    target.End
                )
            });
            lookingUp = true;
            RealizedType realized = aliasedType.UnderlyingType(declaredModule);
            lookingUp = false;
            return realized;
        }
    }
}
