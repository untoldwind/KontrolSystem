using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public static class BuildinFunctions {
        public static readonly IKontrolFunction Some = new CompiledKontrolFunction("Some", "Wrap a value as defined optional", false, new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T")) }, new OptionType(new GenericParameter("T")), typeof(Option).GetMethod("Some"));

        public static readonly IKontrolFunction None = new CompiledKontrolFunction("Some", "Wrap a value as defined optional", false, new List<RealizedParameter> { }, new OptionType(new GenericParameter("T")), typeof(Option).GetMethod("None"));

        public static readonly IKontrolFunction Cell = new CompiledKontrolFunction("Cell", "Wrap a value as cell", false, new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T")) }, BuildinType.Cell, typeof(Cell).GetMethod("Create"));

        public static readonly IKontrolFunction ArrayBuilder = new CompiledKontrolFunction("ArrayBuilder", "Create a new ArrayBuilder", false, new List<RealizedParameter> { new RealizedParameter("capacity", BuildinType.Int, new IntDefaultValue(32)) }, BuildinType.ArrayBuilder, typeof(ArrayBuilder).GetMethod("Create"));

        public static readonly Dictionary<string, IKontrolFunction> ByName = new Dictionary<string, IKontrolFunction> {
            { "Some", Some },
            { "None", None },
            { "Cell", Cell },
            { "ArrayBuilder", ArrayBuilder },
        };
    }
}
