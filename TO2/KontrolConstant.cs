using KontrolSystem.TO2.AST;
using System.Reflection;

namespace KontrolSystem.TO2 {
    public interface IKontrolConstant {
        IKontrolModule Module { get; }

        string Name { get; }

        string Description { get; }

        TO2Type Type { get; }

        FieldInfo RuntimeField { get; }
    }

    public class CompiledKontrolConstant : IKontrolConstant {
        private CompiledKontrolModule module;
        private readonly string name;
        private readonly string description;
        private readonly TO2Type type;
        private readonly FieldInfo runtimeField;

        public CompiledKontrolConstant(string name, string description, TO2Type type, FieldInfo runtimeField) {
            this.name = name;
            this.description = description;
            this.type = type;
            this.runtimeField = runtimeField;
        }

        public IKontrolModule Module => module;
        public string Name => name;

        public string Description => description;

        public TO2Type Type => type;

        public FieldInfo RuntimeField => runtimeField;

        internal void SetModule(CompiledKontrolModule module) => this.module = module;
    }

    public class DeclaredKontrolConstant : IKontrolConstant {
        private readonly DeclaredKontrolModule module;
        public readonly ConstDeclaration to2Constant;
        public readonly FieldInfo runtimeField;

        public DeclaredKontrolConstant(DeclaredKontrolModule module, ConstDeclaration to2Constant,
            FieldInfo runtimeField) {
            this.module = module;
            this.to2Constant = to2Constant;
            this.runtimeField = runtimeField;
        }

        public IKontrolModule Module => module;

        public string Name => to2Constant.name;

        public string Description => to2Constant.description;

        public TO2Type Type => to2Constant.type;

        public FieldInfo RuntimeField => runtimeField;

        public bool IsPublic => to2Constant.isPublic;
    }
}
