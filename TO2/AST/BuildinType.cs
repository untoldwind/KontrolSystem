using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

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

        public static RealizedType ArrayBuilder = new BoundType("", "ArrayBuilder",
            "Helper to create an array of initially unknown size", typeof(ArrayBuilder<>),
            NO_OPERATORS,
            new OperatorCollection {
                 {Operator.AddAssign, new StaticMethodOperatorEmitter(() => new GenericParameter("T"), () => BuildinType.ArrayBuilder, typeof(ArrayBuilderOps).GetMethod("AddTo"), new OpCode[0])}
            },
            new List<(string name, IMethodInvokeFactory invoker)> {
                ("append", new BoundMethodInvokeFactory("Append an element to the array", () => BuildinType.ArrayBuilder, () => new List<RealizedParameter> { new RealizedParameter("element", new GenericParameter("T")) }, false, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Append"))),
                ("result", new BoundMethodInvokeFactory("Build the resulting array", () => new ArrayType(new GenericParameter("T")), () => new List<RealizedParameter> { }, false, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Result"))),
            },
            new List<(string name, IFieldAccessFactory access)> {
                ("length", new BoundPropertyLikeFieldAccessFactory("", () => BuildinType.Int, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetProperty("Length").GetMethod, new OpCode[0]))
            }
        );

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
            //            case "ArrayBuilder" when typeArguments.Count == 1: return new ArrayBuilderType(typeArguments[0]);
            default: return null;
            }
        }
    }
}
