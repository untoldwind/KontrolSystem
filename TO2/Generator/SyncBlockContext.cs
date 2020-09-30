using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Generator {
    public class SyncBlockContext : IBlockContext {
        private readonly SyncBlockContext parent;
        private readonly ModuleContext moduleContext;
        private readonly MethodBuilder methodBuilder;
        private readonly TO2Type expectedReturn;
        private readonly IILEmitter il;
        private readonly List<StructuralError> errors;
        private readonly (LabelRef start, LabelRef end)? innerLoop;
        private VariableResolver externalVariables;
        private readonly Dictionary<string, IBlockVariable> variables;

        private SyncBlockContext(SyncBlockContext parent, IILEmitter il, (LabelRef start, LabelRef end)? innerLoop) {
            this.parent = parent;
            moduleContext = this.parent.ModuleContext;
            methodBuilder = this.parent.methodBuilder;
            expectedReturn = this.parent.expectedReturn;
            externalVariables = this.parent.externalVariables;
            this.il = il;
            variables = this.parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = this.parent.errors;
            this.innerLoop = innerLoop;
        }

        public SyncBlockContext(ModuleContext moduleContext, ConstructorBuilder constructorBuilder) {
            parent = null;
            this.moduleContext = moduleContext;
            methodBuilder = null;
            expectedReturn = BuildinType.Unit;
            il = moduleContext.constructorEmitter;
            variables = new Dictionary<string, IBlockVariable>();
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        public SyncBlockContext(ModuleContext moduleContext, FunctionModifier modifier, bool isAsync, string methodName,
            TO2Type returnType, IEnumerable<FunctionParameter> parameters) {
            parent = null;
            this.moduleContext = moduleContext;
            methodBuilder = this.moduleContext.typeBuilder.DefineMethod(methodName,
                modifier == FunctionModifier.Private
                    ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static
                    : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static,
                isAsync
                    ? this.moduleContext.FutureTypeOf(returnType).future
                    : returnType.GeneratedType(this.moduleContext),
                parameters.Select(parameter => parameter.type.GeneratedType(this.moduleContext)).ToArray());
            expectedReturn = returnType;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
                new MethodParameter(p.name, p.type.UnderlyingType(this.moduleContext), idx)).ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        // LambdaImpl only!!
        public SyncBlockContext(ModuleContext moduleContext, string methodName, TO2Type returnType,
            IEnumerable<FunctionParameter> parameters) {
            parent = null;
            this.moduleContext = moduleContext;
            methodBuilder = this.moduleContext.typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public | MethodAttributes.HideBySig,
                returnType.GeneratedType(this.moduleContext),
                parameters.Select(parameter => parameter.type.GeneratedType(this.moduleContext)).ToArray());
            expectedReturn = returnType;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
                    new MethodParameter(p.name, p.type.UnderlyingType(this.moduleContext), idx + 1))
                .ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        public ModuleContext ModuleContext => moduleContext;

        public MethodBuilder MethodBuilder => methodBuilder;

        public IILEmitter IL => il;

        public TO2Type ExpectedReturn => expectedReturn;

        public bool IsAsync => false;

        public void AddError(StructuralError error) => errors.Add(error);

        public bool HasErrors => errors.Count > 0;

        public List<StructuralError> AllErrors => errors;

        public (LabelRef start, LabelRef end)? InnerLoop => innerLoop;

        public IBlockContext CreateChildContext() => new SyncBlockContext(this, IL, innerLoop);

        public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) =>
            new SyncBlockContext(this, IL, (start, end));

        public IBlockContext CloneCountingContext() =>
            new SyncBlockContext(this, new CountingILEmitter(IL.LastLocalIndex), innerLoop);

        public IBlockVariable MakeTempVariable(RealizedType TO2Type) {
            Type type = TO2Type.GeneratedType(moduleContext);
            ILocalRef localRef = IL.TempLocal(type);

            return new TempVariable(TO2Type, localRef);
        }

        public void SetExternVariables(VariableResolver externalVariables) =>
            this.externalVariables = externalVariables;

        public IBlockVariable FindVariable(string name) => variables.Get(name) ?? externalVariables?.Invoke(name);

        public ILocalRef DeclareHiddenLocal(Type rawType) => il.DeclareLocal(rawType);

        public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
            ILocalRef localRef = IL.DeclareLocal(to2Type.GeneratedType(moduleContext));
            DeclaredVariable variable = new DeclaredVariable(name, isConst, to2Type, localRef);

            variables.Add(name, variable);

            return variable;
        }

        public void RegisterAsyncResume(TO2Type returnType) {
        }
    }
}
