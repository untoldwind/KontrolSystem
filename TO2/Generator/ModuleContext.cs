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
        public readonly IILEmitter constructorEmitter;
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
            moduleAliases = new Dictionary<string, string>();
            mappedTypes = new Dictionary<string, TO2Type> {
                { "ArrayBuilder", BuildinType.ArrayBuilder },
                { "Cell", BuildinType.Cell }
            };
            exportedTypes = new List<(string alias, TO2Type type)>();
            mappedConstants = new Dictionary<string, IKontrolConstant>();
            mappedFunctions = new Dictionary<string, IKontrolFunction>();
            subTypes = new Dictionary<string, TypeBuilder>();

            constructorBuilder = typeBuilder.DefineTypeInitializer();
            constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());
        }

        internal ModuleContext(ModuleContext _parent, string subTypeName, Type parentType, Type[] interfaces) {
            root = _parent.root;
            moduleName = _parent.moduleName;
            typeBuilder = _parent.typeBuilder.DefineNestedType(subTypeName, TypeAttributes.Public | TypeAttributes.NestedPublic, parentType, interfaces);
            moduleAliases = _parent.moduleAliases;
            mappedTypes = _parent.mappedTypes;
            mappedConstants = _parent.mappedConstants;
            mappedFunctions = _parent.mappedFunctions;
            subTypes = _parent.subTypes;
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
