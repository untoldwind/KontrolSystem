using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IModuleItem {
        Position Start {
            get;
        }

        Position End {
            get;
        }

        IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context);

        IEnumerable<StructuralError> TryImportTypes(ModuleContext context);

        IEnumerable<StructuralError> TryImportConstants(ModuleContext context);

        IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context);

        IEnumerable<StructuralError> TryImportFunctions(ModuleContext context);
    }

    public class TO2Module {
        public readonly string name;
        public readonly string description;
        public readonly List<IModuleItem> items;
        public readonly List<UseDeclaration> uses;
        public readonly List<FunctionDeclaration> functions;

        public TO2Module(string _name, string _description, List<IModuleItem> _items) {
            name = _name;
            description = _description;
            items = _items;
            uses = items.Where(item => item is UseDeclaration).Cast<UseDeclaration>().ToList();
            functions = items.Where(item => item is FunctionDeclaration).Cast<FunctionDeclaration>().ToList();
        }

        public IEnumerable<string> Dependencies => uses.Select(u => u.fromModule);

        public List<StructuralError> TryDeclareTypes(ModuleContext context) => items.SelectMany(item => item.TryDeclareTypes(context)).ToList();
        public List<StructuralError> TryImportTypes(ModuleContext context) => items.SelectMany(item => item.TryImportTypes(context)).ToList();
        public List<StructuralError> TryImportConstants(ModuleContext context) => items.SelectMany(item => item.TryImportConstants(context)).ToList();
        public List<StructuralError> TryVerifyFunctions(ModuleContext context) => items.SelectMany(item => item.TryVerifyFunctions(context)).ToList();
        public List<StructuralError> TryImportFunctions(ModuleContext context) => items.SelectMany(item => item.TryImportFunctions(context)).ToList();

        public static string BuildName(string fileName) {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".to2")) fileName = fileName.Substring(0, fileName.Length - 4);
            return Regex.Replace(Regex.Replace(fileName, "[^A-Za-z0-9_\\\\/]", "_"), "[\\\\/]", "::");
        }
    }
}
