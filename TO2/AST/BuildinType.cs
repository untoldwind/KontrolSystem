using System.Collections.Generic;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuildinType : RealizedType {
        public static OperatorCollection NO_OPERATORS = new OperatorCollection();
        public static Dictionary<string, IMethodInvokeFactory> NO_METHODS = new Dictionary<string, IMethodInvokeFactory>();
        public static Dictionary<string, IFieldAccessFactory> NO_FIELDS = new Dictionary<string, IFieldAccessFactory>();

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public static RealizedType Unit = new TO2Unit();
        public static RealizedType Bool = new TO2Bool();
        public static RealizedType Int = new TO2Int();
        public static RealizedType Float = new TO2Float();
        public static RealizedType String = new TO2SString();
        public static RealizedType Range = new RangeType();

        public static TO2Type GetBuildinType(List<string> namePath, List<TO2Type> typeArguments) {
            if (namePath.Count != 1) return null;

            switch (namePath[0]) {
            case "Unit" when typeArguments.Count == 0: return Unit;
            case "bool" when typeArguments.Count == 0: return Bool;
            case "int" when typeArguments.Count == 0: return Int;
            case "float" when typeArguments.Count == 0: return Float;
            case "string" when typeArguments.Count == 0: return String;
            case "Range" when typeArguments.Count == 0: return Range;
            case "Option" when typeArguments.Count == 1: return new OptionType(typeArguments[0]);
            case "Cell" when typeArguments.Count == 1: return new CellType(typeArguments[0]);
            case "Result" when typeArguments.Count == 2: return new ResultType(typeArguments[0], typeArguments[1]);
            case "ArrayBuilder" when typeArguments.Count == 1: return new ArrayBuilderType(typeArguments[0]);
            default: return null;
            }
        }
    }
}
