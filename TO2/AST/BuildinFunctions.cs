using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public static class BuildinFunctions {
        public static readonly IKontrolFunction Some = new CompiledKontrolFunction("Some", "Wrap a value as defined optional", false, new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T"))}, new OptionType(new GenericParameter("T")), typeof(Option).GetMethod("Some"));

        public static readonly IKontrolFunction Cell = new CompiledKontrolFunction("Cell", "Wrap a value as cell", false, new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T"))}, BuildinType.Cell, typeof(Cell).GetMethod("Create"));

        public static readonly Dictionary<string, IKontrolFunction> ByName = new Dictionary<string, IKontrolFunction> {
            { "Some", Some },
//            { "Cell", Cell },
        };
    }
}
