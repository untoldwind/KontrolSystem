using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public struct AsyncResume {
        internal readonly int state;
        internal readonly LabelRef resumeLabel;
        internal readonly LabelRef pollLabel;
        internal readonly RealizedType returnType;
        internal readonly FieldInfo futureField;
        internal readonly ILocalRef futureResultVar;

        internal AsyncResume(int state, LabelRef resumeLabel, LabelRef pollLabel, RealizedType returnType, FieldInfo futureField, ILocalRef futureResultVar) {
            this.state = state;
            this.resumeLabel = resumeLabel;
            this.pollLabel = pollLabel;
            this.returnType = returnType;
            this.futureField = futureField;
            this.futureResultVar = futureResultVar;
        }

        internal void EmitPoll(AsyncBlockContext context) {
            context.IL.MarkLabel(pollLabel);
            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldfld, futureField);
            context.IL.EmitCall(OpCodes.Callvirt, futureField.FieldType.GetMethod("PollValue"), 1);
            futureResultVar.EmitStore(context);
            futureResultVar.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldfld, futureResultVar.LocalType.GetField("ready"));
            context.IL.Emit(OpCodes.Brfalse, context.notReady);
            context.IL.Emit(OpCodes.Br, context.resume);
        }
    }

    public struct StateRef {
        internal readonly ILocalRef localRef;
        internal readonly Type type;
        internal readonly FieldInfo stoageField;

        internal StateRef(ILocalRef localRef, Type type, FieldInfo storageField) {
            this.localRef = localRef;
            this.type = type;
            stoageField = storageField;
        }

        internal void EmitStore(AsyncBlockContext context) {
            context.IL.Emit(OpCodes.Ldarg_0);
            localRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, stoageField);
        }

        internal void EmitRestore(AsyncBlockContext context) {
            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldfld, stoageField);
            localRef.EmitStore(context);
        }
    }

    public class AsyncBlockContext : IBlockContext {
        private readonly Context root;
        private readonly AsyncBlockContext parent;
        private readonly ModuleContext moduleContext;
        private readonly MethodBuilder methodBuilder;
        private readonly TO2Type expectedReturn;
        private readonly IILEmitter il;
        private readonly List<StructuralError> errors;
        private readonly (LabelRef start, LabelRef end)? innerLoop;
        public readonly Dictionary<string, IBlockVariable> variables;
        internal readonly FieldInfo stateField;
        internal readonly LabelRef storeState;
        internal readonly LabelRef notReady;
        internal readonly LabelRef resume;
        internal readonly List<AsyncResume> asyncResumes;
        internal readonly List<StateRef> stateRefs;

        private AsyncBlockContext(AsyncBlockContext parent, (LabelRef start, LabelRef end)? innerLoop) {
            this.parent = parent;
            root = this.parent.root;
            moduleContext = this.parent.ModuleContext;
            methodBuilder = this.parent.methodBuilder;
            expectedReturn = this.parent.expectedReturn;
            il = this.parent.il;
            variables = this.parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = this.parent.errors;
            this.innerLoop = innerLoop;
            asyncResumes = this.parent.asyncResumes;
            stateRefs = this.parent.stateRefs;
            stateField = this.parent.stateField;
            storeState = this.parent.storeState;
            notReady = this.parent.notReady;
            resume = this.parent.resume;
        }

        private AsyncBlockContext(AsyncBlockContext parent, IILEmitter il, (LabelRef start, LabelRef end)? innerLoop) {
            this.parent = parent;
            root = this.parent.root;
            moduleContext = this.parent.ModuleContext;
            methodBuilder = this.parent.methodBuilder;
            expectedReturn = this.parent.expectedReturn;
            this.il = il;
            variables = this.parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = this.parent.errors;
            this.innerLoop = innerLoop;
            stateField = this.parent.stateField;
            asyncResumes = null;
            stateRefs = null;
            storeState = this.parent.storeState;
            notReady = this.parent.notReady;
            resume = this.parent.resume;
        }

        public AsyncBlockContext(ModuleContext moduleContext, FunctionModifier modifier, string methodName, TO2Type expectedReturn, Type generatedReturn, IEnumerable<IBlockVariable> parameters) {
            parent = null;
            this.moduleContext = moduleContext;
            root = this.moduleContext.root;
            methodBuilder = this.moduleContext.typeBuilder.DefineMethod(methodName,
                            modifier == FunctionModifier.Private ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                            generatedReturn,
                            new Type[0]);
            this.expectedReturn = expectedReturn;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
            stateField = this.moduleContext.typeBuilder.DefineField("<async>_state", typeof(int), FieldAttributes.Private);
            asyncResumes = new List<AsyncResume>();
            stateRefs = new List<StateRef>();
            storeState = il.DefineLabel(false);
            notReady = il.DefineLabel(false);
            resume = il.DefineLabel(false);
        }

        public ModuleContext ModuleContext => moduleContext;

        public MethodBuilder MethodBuilder => methodBuilder;

        public IILEmitter IL => il;

        public TO2Type ExpectedReturn => expectedReturn;

        public bool IsAsync => true;

        public void AddError(StructuralError error) => errors.Add(error);

        public bool HasErrors => errors.Count > 0;

        public List<StructuralError> AllErrors => errors;

        public (LabelRef start, LabelRef end)? InnerLoop => innerLoop;

        public IBlockContext CreateChildContext() => new AsyncBlockContext(this, innerLoop);

        public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) => new AsyncBlockContext(this, (start, end));

        public IBlockContext CloneCountingContext() => new AsyncBlockContext(this, new CountingILEmitter(IL.LastLocalIndex), innerLoop);

        public IBlockVariable MakeTempVariable(RealizedType to2Type) {
            Type type = to2Type.GeneratedType(moduleContext);
            ILocalRef localRef = il.TempLocal(type);

            return new TempVariable(to2Type, localRef);
        }

        public IBlockVariable FindVariable(string name) => variables.Get(name);

        public ILocalRef DeclareHiddenLocal(Type rawType) {
            ILocalRef localRef = il.DeclareLocal(rawType);

            if (stateRefs != null) {
                FieldBuilder storeField = moduleContext.typeBuilder.DefineField($"<async>_store_{stateRefs.Count}", rawType, FieldAttributes.Private);
                stateRefs.Add(new StateRef(localRef, rawType, storeField));
            }

            return localRef;
        }

        public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
            Type type = to2Type.GeneratedType(moduleContext);
            ILocalRef localRef = il.DeclareLocal(type);
            DeclaredVariable variable = new DeclaredVariable(name, isConst, to2Type, localRef);

            variables.Add(name, variable);

            if (stateRefs != null) {
                FieldBuilder storeField = moduleContext.typeBuilder.DefineField($"<async>_store_{stateRefs.Count}", type, FieldAttributes.Private);
                stateRefs.Add(new StateRef(localRef, type, storeField));
            }
            return variable;
        }

        public void RegisterAsyncResume(TO2Type returnType) {
            int state = (asyncResumes?.Count ?? 0) + 1;

            (Type futureType, Type futureResultType) = moduleContext.FutureTypeOf(returnType);
            FieldInfo futureField = asyncResumes != null ? moduleContext.typeBuilder.DefineField($"<async>_future_{state}", futureType, FieldAttributes.Private) : null;
            ILocalRef futureResultVar = IL.DeclareLocal(futureResultType);
            ILocalRef futureTemp = IL.TempLocal(futureType);
            futureTemp.EmitStore(this);
            il.Emit(OpCodes.Ldarg_0);
            futureTemp.EmitLoad(this);
            il.Emit(OpCodes.Stfld, futureField);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, state);
            il.Emit(OpCodes.Stfld, stateField);
            il.Emit(OpCodes.Br, storeState);

            LabelRef resumeLabel = il.DefineLabel(false);
            il.MarkLabel(resumeLabel);
            futureResultVar.EmitLoad(this);
            il.Emit(OpCodes.Ldfld, futureResultVar.LocalType.GetField("value"));

            asyncResumes?.Add(new AsyncResume(state, resumeLabel, il.DefineLabel(false), returnType.UnderlyingType(moduleContext), futureField, futureResultVar));
        }
    }
}
