using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public abstract class TO2Type {
        public abstract string Name {
            get;
        }

        public virtual string Description => "";

        public virtual string LocalName => Name;

        public virtual bool IsValid(ModuleContext context) => true;

        public abstract RealizedType UnderlyingType(ModuleContext context);

        public abstract Type GeneratedType(ModuleContext context);

        public abstract IOperatorCollection AllowedPrefixOperators(ModuleContext context);

        public abstract IOperatorCollection AllowedSuffixOperators(ModuleContext context);

        public abstract IMethodInvokeFactory FindMethod(ModuleContext context, string methodName);

        public abstract IFieldAccessFactory FindField(ModuleContext context, string fieldName);

        public abstract IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec);

        public virtual IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => null;

        public virtual bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => GeneratedType(context).IsAssignableFrom(otherType.GeneratedType(context));

        public virtual IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => DefaultAssignEmitter.Instance;

        public override string ToString() => Name;
    }

    public abstract class RealizedType : TO2Type {
        public abstract Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) => DeclaredMethods.Get(methodName);

        public abstract Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) => DeclaredFields.Get(fieldName);
    }
}
