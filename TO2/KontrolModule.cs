using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2 {
    public interface IKontrolModule {
        string Name {
            get;
        }

        string Description { get; }

        bool IsCompiled {
            get;
        }

        Type RuntimeType {
            get;
        }

        ConstructorInfo RuntimeConstructor {
            get;
        }

        IEnumerable<string> AllTypeNames {
            get;
        }

        RealizedType FindType(string name);

        IEnumerable<string> AllConstantNames {
            get;
        }

        IKontrolConstant FindConstant(string name);

        IEnumerable<string> AllFunctionNames {
            get;
        }

        IKontrolFunction FindFunction(string name);

        IEnumerable<IKontrolFunction> TestFunctions {
            get;
        }
    }

    public class CompiledKontrolModule : IKontrolModule {
        private readonly string name;
        private readonly string description;
        private readonly Type runtimeType;
        private readonly ConstructorInfo runtimeConstructor;
        private readonly Dictionary<string, CompiledKontrolFunction> publicFunctions;
        private readonly List<CompiledKontrolFunction> testFunctions;
        private readonly Dictionary<string, RealizedType> types;
        private readonly Dictionary<string, IKontrolConstant> constants;

        public CompiledKontrolModule(string _name,
                                     string _description,
                                     Type _runtimeType,
                                     IEnumerable<(string alias, RealizedType type)> _types,
                                     IEnumerable<IKontrolConstant> _constants,
                                     IEnumerable<CompiledKontrolFunction> _functions,
                                     List<CompiledKontrolFunction> _testFunctions) {
            name = _name;
            description = _description;
            runtimeType = _runtimeType;
            constants = _constants.ToDictionary(constant => constant.Name);
            publicFunctions = _functions.ToDictionary(function => function.Name);
            testFunctions = _testFunctions;
            types = _types.ToDictionary(t => t.alias, t => t.type);

            if (runtimeType != null) {
                runtimeConstructor = runtimeType.GetConstructor(new Type[] { typeof(IContext), typeof(Dictionary<string, object>) });

                if (runtimeConstructor == null) throw new ArgumentException($"{runtimeType} does not have a context constructor");
            }

            foreach (CompiledKontrolFunction function in _functions) function.SetModule(this);
            foreach (CompiledKontrolFunction function in _testFunctions) function.SetModule(this);
        }

        public string Name => name;

        public string Description => description;

        public bool IsCompiled => true;

        public Type RuntimeType => runtimeType;

        public ConstructorInfo RuntimeConstructor => runtimeConstructor;

        public IEnumerable<string> AllTypeNames => types.Keys;

        public RealizedType FindType(string name) => types.Get(name);

        public IEnumerable<string> AllConstantNames => constants.Keys;

        public IKontrolConstant FindConstant(string name) => constants.Get(name);

        public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

        public IKontrolFunction FindFunction(string name) => publicFunctions.Get(name);

        public IEnumerable<IKontrolFunction> TestFunctions => testFunctions;

        public void RegisterType(BoundType TO2Type) => types.Add(TO2Type.localName, TO2Type);
    }

    public class DeclaredKontrolModule : IKontrolModule {
        private readonly string name;
        private readonly string description;
        public readonly Dictionary<string, TO2Type> publicTypes;
        public readonly Dictionary<string, IKontrolFunction> publicFunctions;
        public readonly List<DeclaredKontrolFunction> declaredFunctions;
        public readonly ModuleContext moduleContext;
        public readonly TO2Module to2Module;

        public DeclaredKontrolModule(string _name, string _description, ModuleContext _moduleContext, TO2Module _to2Module, IEnumerable<(string alias, TO2Type type)> _types) {
            name = _name;
            description = _description;
            moduleContext = _moduleContext;
            to2Module = _to2Module;
            publicTypes = _types.ToDictionary(t => t.alias, t => t.type);
            publicFunctions = new Dictionary<string, IKontrolFunction>();
            declaredFunctions = new List<DeclaredKontrolFunction>();
        }

        public string Name => name;

        public string Description => description;

        public bool IsCompiled => true;

        public Type RuntimeType => moduleContext.typeBuilder.AsType();

        public ConstructorInfo RuntimeConstructor => moduleContext.constructorBuilder;

        public IEnumerable<string> AllTypeNames => Enumerable.Empty<string>();

        public RealizedType FindType(string name) => publicTypes.Get(name)?.UnderlyingType(moduleContext);

        public IEnumerable<string> AllConstantNames => Enumerable.Empty<string>();

        public IKontrolConstant FindConstant(string name) => null;

        public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

        public IKontrolFunction FindFunction(string name) => publicFunctions.Get(name);

        public IEnumerable<IKontrolFunction> TestFunctions => declaredFunctions.Where(f => f.to2Function.modifier == FunctionModifier.Test);
    }
}
