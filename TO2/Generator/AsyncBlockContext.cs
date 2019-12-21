using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Generator {
    public struct AsyncResume {
        internal readonly int state;
        internal readonly LabelRef resumeLabel;
        internal readonly LabelRef pollLabel;
        internal readonly RealizedType returnType;
        internal readonly FieldInfo futureField;
        internal readonly ILocalRef futureResultVar;

        internal AsyncResume(int _state, LabelRef _resumeLabel, LabelRef _pollLabel, RealizedType _returnType, FieldInfo _futureField, ILocalRef _futureResultVar) {
            state = _state;
            resumeLabel = _resumeLabel;
            pollLabel = _pollLabel;
            returnType = _returnType;
            futureField = _futureField;
            futureResultVar = _futureResultVar;
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

        internal StateRef(ILocalRef _localRef, Type _type, FieldInfo _storageField) {
            localRef = _localRef;
            type = _type;
            stoageField = _storageField;
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

        private AsyncBlockContext(AsyncBlockContext _parent, (LabelRef start, LabelRef end)? _innerLoop) {
            parent = _parent;
            root = parent.root;
            moduleContext = parent.ModuleContext;
            methodBuilder = parent.methodBuilder;
            expectedReturn = parent.expectedReturn;
            il = parent.il;
            variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = parent.errors;
            innerLoop = _innerLoop;
            asyncResumes = parent.asyncResumes;
            stateRefs = parent.stateRefs;
            stateField = parent.stateField;
            storeState = parent.storeState;
            notReady = parent.notReady;
            resume = parent.resume;
        }

        private AsyncBlockContext(AsyncBlockContext _parent, IILEmitter _il, (LabelRef start, LabelRef end)? _innerLoop) {
            parent = _parent;
            root = parent.root;
            moduleContext = parent.ModuleContext;
            methodBuilder = parent.methodBuilder;
            expectedReturn = parent.expectedReturn;
            il = _il;
            variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = parent.errors;
            innerLoop = _innerLoop;
            stateField = parent.stateField;
            asyncResumes = null;
            stateRefs = null;
            storeState = parent.storeState;
            notReady = parent.notReady;
            resume = parent.resume;
        }

        public AsyncBlockContext(ModuleContext _moduleContext, FunctionModifier modifier, string methodName, TO2Type _expectedReturn, Type generatedReturn, IEnumerable<IBlockVariable> parameters) {
            parent = null;
            moduleContext = _moduleContext;
            root = moduleContext.root;
            methodBuilder = moduleContext.typeBuilder.DefineMethod(methodName,
                            modifier == FunctionModifier.Private ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                            generatedReturn,
                            new Type[0]);
            expectedReturn = _expectedReturn;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
            stateField = moduleContext.typeBuilder.DefineField("<async>_state", typeof(int), FieldAttributes.Private);
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
            if (returnType != BuildinType.Unit) {
                futureResultVar.EmitLoad(this);
                il.Emit(OpCodes.Ldfld, futureResultVar.LocalType.GetField("value"));
            }

            asyncResumes?.Add(new AsyncResume(state, resumeLabel, il.DefineLabel(false), returnType.UnderlyingType(moduleContext), futureField, futureResultVar));
        }
    }
}
