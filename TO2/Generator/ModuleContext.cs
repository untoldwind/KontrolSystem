using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Generator {
    public class ModuleContext {
        public readonly string moduleName;
        public readonly Context root;
        public readonly TypeBuilder typeBuilder;
        public readonly ConstructorBuilder constructorBuilder;
        public readonly FieldInfo contextField;
        public readonly IILEmitter constructorEmitter;
        private readonly SortedDictionary<string, (IKontrolModule module, FieldInfo moduleField)> importedModules;
        public readonly Dictionary<string, string> moduleAliases;
        public readonly Dictionary<string, TO2Type> mappedTypes;
        public readonly List<(string alias, TO2Type type)> exportedTypes;
        public readonly Dictionary<string, IKontrolConstant> mappedConstants;
        public readonly Dictionary<string, IKontrolFunction> mappedFunctions;
        private readonly Dictionary<string, TypeBuilder> subTypes;

        internal ModuleContext(Context _root, string _moduleName) {
            root = _root;
            moduleName = _moduleName;
            typeBuilder = root.moduleBuilder.DefineType(moduleName.ToUpperInvariant().Replace(':', '_'), TypeAttributes.Public);
            importedModules = new SortedDictionary<string, (IKontrolModule module, FieldInfo moduleField)>();
            moduleAliases = new Dictionary<string, string>();
            mappedTypes = new Dictionary<string, TO2Type>();
            exportedTypes = new List<(string alias, TO2Type type)>();
            mappedConstants = new Dictionary<string, IKontrolConstant>();
            mappedFunctions = new Dictionary<string, IKontrolFunction>();
            subTypes = new Dictionary<string, TypeBuilder>();

            constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(IContext), typeof(Dictionary<string, object>) });
            contextField = typeBuilder.DefineField("context", typeof(IContext), FieldAttributes.Public | FieldAttributes.InitOnly);

            constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());
            constructorEmitter.Emit(OpCodes.Ldarg_0);
            constructorEmitter.Emit(OpCodes.Ldarg_1);
            constructorEmitter.Emit(OpCodes.Stfld, contextField);
            constructorEmitter.Emit(OpCodes.Ldarg_2);
            constructorEmitter.Emit(OpCodes.Ldstr, moduleName);
            constructorEmitter.Emit(OpCodes.Ldarg_0);
            constructorEmitter.EmitCall(OpCodes.Call, typeof(Dictionary<string, object>).GetMethod("Add"), 3);

            ConstructorBuilder defaultConstructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(IContext) });

            IILEmitter defaultIL = new GeneratorILEmitter(defaultConstructor.GetILGenerator());

            defaultIL.Emit(OpCodes.Ldarg_0);
            defaultIL.Emit(OpCodes.Ldarg_1);
            defaultIL.EmitNew(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(new Type[0]), 0, 1);
            defaultIL.EmitNew(OpCodes.Call, constructorBuilder, 3, 0);
            defaultIL.EmitReturn(typeof(void));
        }

        internal ModuleContext(ModuleContext _parent, string subTypeName, Type parentType, Type[] interfaces) {
            root = _parent.root;
            moduleName = _parent.moduleName;
            typeBuilder = _parent.typeBuilder.DefineNestedType(subTypeName, TypeAttributes.Public | TypeAttributes.NestedPublic, parentType, interfaces);
            importedModules = new SortedDictionary<string, (IKontrolModule module, FieldInfo moduleField)>();
            moduleAliases = _parent.moduleAliases;
            mappedTypes = _parent.mappedTypes;
            mappedConstants = _parent.mappedConstants;
            mappedFunctions = _parent.mappedFunctions;
            subTypes = _parent.subTypes;

            contextField = typeBuilder.DefineField("context", typeof(IContext), FieldAttributes.Public | FieldAttributes.InitOnly);
        }

        public Type CreateType() {
            constructorEmitter?.EmitReturn(typeof(void));
            foreach (TypeBuilder subType in subTypes.Values) subType.CreateType();
            Type type = typeBuilder.CreateType();
#if TRIGGER_JIT
            foreach(MethodInfo method in type.GetMethods()) System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
#endif
            return type;
        }

        public IEnumerable<(IKontrolModule module, FieldInfo moduleField)> ImportedModules => importedModules.Values;

        public FieldInfo RegisterImportedModule(IKontrolModule module) {
            if (module.RuntimeType == null) return null; // Modules without a runtime type are statically bound and do not need treatment

            if (importedModules.ContainsKey(module.Name)) return importedModules[module.Name].moduleField;

            FieldBuilder moduleField = typeBuilder.DefineField(module.Name.ToLower().Replace(':', '_'), module.RuntimeType, FieldAttributes.Public | FieldAttributes.InitOnly);
            importedModules.Add(module.Name, (module, moduleField));

            if (constructorEmitter == null) return moduleField;

            constructorEmitter.Emit(OpCodes.Ldarg_0);

            constructorEmitter.Emit(OpCodes.Ldarg_2);
            constructorEmitter.Emit(OpCodes.Ldstr, module.Name);
            constructorEmitter.EmitCall(OpCodes.Call, typeof(Dictionary<string, object>).GetMethod("ContainsKey"), 2);
            LabelRef createNew = constructorEmitter.DefineLabel(true);
            constructorEmitter.Emit(OpCodes.Brfalse_S, createNew);

            constructorEmitter.Emit(OpCodes.Ldarg_2);
            constructorEmitter.Emit(OpCodes.Ldstr, module.Name);
            constructorEmitter.EmitCall(OpCodes.Call, typeof(Dictionary<string, object>).GetMethod("get_Item"), 2);

            LabelRef setField = constructorEmitter.DefineLabel(true);
            constructorEmitter.Emit(OpCodes.Br_S, setField);

            constructorEmitter.MarkLabel(createNew);
            constructorEmitter.Emit(OpCodes.Ldarg_1);
            constructorEmitter.Emit(OpCodes.Ldarg_2);
            constructorEmitter.EmitNew(OpCodes.Newobj, module.RuntimeConstructor, 2, 1);

            constructorEmitter.MarkLabel(setField);
            constructorEmitter.AdjustStack(-1);
            constructorEmitter.Emit(OpCodes.Stfld, moduleField);

            return moduleField;
        }

        public IKontrolModule FindModule(string moduleName) => moduleAliases.ContainsKey(moduleName) ? root.registry.modules.Get(moduleAliases[moduleName]) : root.registry.modules.Get(moduleName);

        public IBlockContext CreateMethodContext(FunctionModifier modifier, bool isAsync, string methodName, TO2Type returnType, IEnumerable<FunctionParameter> parameters) {
            return new SyncBlockContext(this, modifier, isAsync, methodName, returnType, parameters);
        }

        public ModuleContext DefineSubComtext(string name, Type parentType, params Type[] interfaces) {
            ModuleContext subContext = new ModuleContext(this, name, parentType, interfaces);

            subTypes.Add(name, subContext.typeBuilder);

            return subContext;
        }

        public (Type future, Type futureResult) FutureTypeOf(TO2Type to2Type) {
            Type type = to2Type.GeneratedType(this);
            Type parameterType = type == typeof(void) ? typeof(object) : type;

            return (typeof(Future<>).MakeGenericType(parameterType), typeof(FutureResult<>).MakeGenericType(parameterType));
        }
    }
}
