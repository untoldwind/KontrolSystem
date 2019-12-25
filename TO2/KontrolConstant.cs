using KontrolSystem.TO2.AST;
using System.Reflection;

namespace KontrolSystem.TO2 {
    public interface IKontrolConstant {
        IKontrolModule Module {
            get;
        }

        string Name {
            get;
        }

        string Description {
            get;
        }

        TO2Type Type {
            get;
        }

        FieldInfo RuntimeFIeld {
            get;
        }
    }

    public class CompiledKontrolConstant : IKontrolConstant {
        private CompiledKontrolModule module;
        private readonly string name;
        private readonly string description;
        private readonly TO2Type type;
        private readonly FieldInfo runtimeFIeld;

        public CompiledKontrolConstant(string _name, string _description, TO2Type _type, FieldInfo _runtimeField) {
            name = _name;
            description = _description;
            type = _type;
            runtimeFIeld = _runtimeField;
        }

        public IKontrolModule Module => Module;
        public string Name => name;

        public string Description => description;

        public TO2Type Type => type;

        public FieldInfo RuntimeFIeld => runtimeFIeld;

        internal void SetModule(CompiledKontrolModule _module) => module = _module;
    }

    public class DeclaredKontrolConstant : IKontrolConstant {
        private readonly DeclaredKontrolModule module;
        public readonly ConstDeclaration to2Constant;
        public readonly FieldInfo runtimeFIeld;

        public DeclaredKontrolConstant(DeclaredKontrolModule _module, ConstDeclaration _to2Constant, FieldInfo _runtimeFIeld) {
            module = _module;
            to2Constant = _to2Constant;
            runtimeFIeld = _runtimeFIeld;
        }

        public IKontrolModule Module => module;

        public string Name => to2Constant.name;

        public string Description => to2Constant.description;

        public TO2Type Type => to2Constant.type;

        public FieldInfo RuntimeFIeld => runtimeFIeld;

        public bool IsPublic => to2Constant.isPublic;
    }
}
