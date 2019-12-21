using KontrolSystem.TO2.AST;
using System.Reflection;

namespace KontrolSystem.TO2 {
    public interface IKontrolConstant {
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

        public string Name => name;

        public string Description => description;

        public TO2Type Type => type;

        public FieldInfo RuntimeFIeld => runtimeFIeld;
    }
}
