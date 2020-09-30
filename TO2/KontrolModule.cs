using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2 {
    public interface IKontrolModule {
        string Name { get; }

        string Description { get; }

        bool IsCompiled { get; }

        IEnumerable<string> AllTypeNames { get; }

        RealizedType FindType(string name);

        IEnumerable<string> AllConstantNames { get; }

        IKontrolConstant FindConstant(string name);

        IEnumerable<string> AllFunctionNames { get; }

        IKontrolFunction FindFunction(string name);

        IEnumerable<IKontrolFunction> TestFunctions { get; }
    }

    public class CompiledKontrolModule : IKontrolModule {
        private readonly string name;
        private readonly string description;
        private readonly Dictionary<string, CompiledKontrolFunction> publicFunctions;
        private readonly List<CompiledKontrolFunction> testFunctions;
        private readonly Dictionary<string, RealizedType> types;
        private readonly Dictionary<string, CompiledKontrolConstant> constants;

        public CompiledKontrolModule(string name,
            string description,
            IEnumerable<(string alias, RealizedType type)> types,
            IEnumerable<CompiledKontrolConstant> constants,
            IEnumerable<CompiledKontrolFunction> functions,
            List<CompiledKontrolFunction> testFunctions) {
            this.name = name;
            this.description = description;
            this.constants = constants.ToDictionary(constant => constant.Name);
            publicFunctions = functions.ToDictionary(function => function.Name);
            this.testFunctions = testFunctions;
            this.types = types.ToDictionary(t => t.alias, t => t.type);


            foreach (CompiledKontrolConstant constant in constants) constant.SetModule(this);
            foreach (CompiledKontrolFunction function in functions) function.SetModule(this);
            foreach (CompiledKontrolFunction function in testFunctions) function.SetModule(this);
        }

        public string Name => name;

        public string Description => description;

        public bool IsCompiled => true;

        public IEnumerable<string> AllTypeNames => types.Keys;

        public RealizedType FindType(string name) => types.Get(name);

        public IEnumerable<string> AllConstantNames => constants.Keys;

        public IKontrolConstant FindConstant(string name) => constants.Get(name);

        public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

        public IKontrolFunction FindFunction(string name) => publicFunctions.Get(name);

        public IEnumerable<IKontrolFunction> TestFunctions => testFunctions;

        public void RegisterType(BoundType to2Type) => types.Add(to2Type.localName, to2Type);
    }

    public class DeclaredKontrolModule : IKontrolModule {
        private readonly string name;
        private readonly string description;
        private readonly Dictionary<string, TO2Type> publicTypes;
        public readonly Dictionary<string, IKontrolFunction> publicFunctions;
        public readonly List<DeclaredKontrolFunction> declaredFunctions;
        public readonly Dictionary<string, DeclaredKontrolConstant> declaredConstants;
        public readonly ModuleContext moduleContext;
        public readonly TO2Module to2Module;

        public DeclaredKontrolModule(string name, string description, ModuleContext moduleContext, TO2Module to2Module,
            IEnumerable<(string alias, TO2Type type)> types) {
            this.name = name;
            this.description = description;
            this.moduleContext = moduleContext;
            this.to2Module = to2Module;
            publicTypes = types.ToDictionary(t => t.alias, t => t.type);
            publicFunctions = new Dictionary<string, IKontrolFunction>();
            declaredFunctions = new List<DeclaredKontrolFunction>();
            declaredConstants = new Dictionary<string, DeclaredKontrolConstant>();
        }

        public string Name => name;

        public string Description => description;

        public bool IsCompiled => true;

        public IEnumerable<string> AllTypeNames => Enumerable.Empty<string>();

        public RealizedType FindType(string name) => publicTypes.Get(name)?.UnderlyingType(moduleContext);

        public IEnumerable<string> AllConstantNames =>
            declaredConstants.Where(kv => kv.Value.IsPublic).Select(kv => kv.Key);

        public IKontrolConstant FindConstant(string name) => declaredConstants.Get(name);

        public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

        public IKontrolFunction FindFunction(string name) => publicFunctions.Get(name);

        public IEnumerable<IKontrolFunction> TestFunctions =>
            declaredFunctions.Where(f => f.to2Function.modifier == FunctionModifier.Test);
    }
}
