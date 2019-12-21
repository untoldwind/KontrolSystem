using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract class BuildinType : RealizedType {
        public static OperatorCollection NO_OPERATORS = new OperatorCollection();
        public static Dictionary<string, IMethodInvokeFactory> NO_METHODS = new Dictionary<string, IMethodInvokeFactory>();
        public static Dictionary<string, IFieldAccessFactory> NO_FIELDS = new Dictionary<string, IFieldAccessFactory>();

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        internal class KSSUnit : BuildinType {
            public override string Name => "Unit";
            public override Type GeneratedType(ModuleContext context) => typeof(void);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => NO_OPERATORS;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => NO_METHODS;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => NO_FIELDS;
        }

        public static RealizedType Unit = new KSSUnit();

        internal class TO2Bool : BuildinType {
            private OperatorCollection allowedPrefixOperators;
            private OperatorCollection allowedPostfixOperators;
            private Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private Dictionary<string, IFieldAccessFactory> allowedFields;

            internal TO2Bool() {
                allowedPrefixOperators = new OperatorCollection {
                    {Operator.Not, new DirectOperatorEmitter(() => BuildinType.Unit, () => BuildinType.Bool, OpCodes.Ldc_I4_0, OpCodes.Ceq)}
                };
                allowedPostfixOperators = new OperatorCollection {
                    {Operator.Eq, new DirectOperatorEmitter(() => BuildinType.Bool, () => BuildinType.Bool, OpCodes.Ceq)},
                    {Operator.NotEq, new DirectOperatorEmitter(() => BuildinType.Bool, () => BuildinType.Bool, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.BoolAnd, new DirectOperatorEmitter(() => BuildinType.Bool, () => BuildinType.Bool, OpCodes.And)},
                    {Operator.BoolOr, new DirectOperatorEmitter(() => BuildinType.Bool, () => BuildinType.Bool, OpCodes.Or)}
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {"to_string", new BoundMethodInvokeFactory("Convert boolean to string", () => BuildinType.String, () => new List<FunctionParameter>(), false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("BoolToString") )}
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"to_int", new InlineFieldAccessFactory("Value converted to integer (false -> 0, true -> 1)", () => BuildinType.Int, OpCodes.Conv_I8)},
                    {"to_float", new InlineFieldAccessFactory("Value converted to float (flase -> 0.0, true -> 1.0)", () => BuildinType.Float, OpCodes.Conv_R8)},
                };
            }

            public override string Name => "bool";
            public override Type GeneratedType(ModuleContext context) => typeof(bool);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedPostfixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }

        public static RealizedType Bool = new TO2Bool();

        internal class TO2Int : BuildinType {
            private OperatorCollection allowedPrefixOperators;
            private OperatorCollection allowedPostfixOperators;
            private Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private Dictionary<string, IFieldAccessFactory> allowedFields;

            internal TO2Int() {
                allowedPrefixOperators = new OperatorCollection {
                    {Operator.Neg, new DirectOperatorEmitter(() => BuildinType.Unit, () => BuildinType.Int, OpCodes.Neg)},
                };
                allowedPostfixOperators = new OperatorCollection {
                    {Operator.Add, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Add)},
                    {Operator.AddAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Add)},
                    {Operator.Sub, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Sub)},
                    {Operator.SubAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Sub)},
                    {Operator.Mul, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Mul)},
                    {Operator.MulAsign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Mul)},
                    {Operator.Div, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Div)},
                    {Operator.DivAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Div)},
                    {Operator.Mod, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Rem)},
                    {Operator.BitOr, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Or)},
                    {Operator.BitOrAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Or)},
                    {Operator.BitAnd, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.And)},
                    {Operator.BitAndAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.And)},
                    {Operator.BitXor, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Xor)},
                    {Operator.BitXorAssign, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Xor)},
                    {Operator.Eq, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Ceq)},
                    {Operator.NotEq, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Ceq, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.Gt, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Cgt)},
                    {Operator.Lt, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Clt)},
                    {Operator.Ge, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Clt, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.Le, new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Cgt, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {"to_string", new BoundMethodInvokeFactory("Convert integer to string", () => BuildinType.String, () => new List<FunctionParameter>(), false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("IntToString") )}
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"to_bool", new InlineFieldAccessFactory("Value converted to bool (0 -> false, != 0 -> true)", () => BuildinType.Bool, OpCodes.Conv_I4)},
                    {"to_float", new InlineFieldAccessFactory("Value converted to float", () => BuildinType.Float, OpCodes.Conv_R8)},
                    {"abs", new BoundPropertyLikeFieldAccessFactory("Absolute value", () => BuildinType.Int,typeof(Math), typeof(Math).GetMethod("Abs", new Type[] { typeof(long) }))},
                    {"sign", new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)", () => BuildinType.Int,typeof(Math), typeof(Math).GetMethod("Sign", new Type[] { typeof(long) }), OpCodes.Conv_I8)},
                };
            }

            public override string Name => "int";
            public override Type GeneratedType(ModuleContext context) => typeof(long);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedPostfixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }

        public static RealizedType Int = new TO2Int();

        internal class IntToFloatAssign : IAssignEmitter {
            public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
                expression.EmitCode(context, false);
                context.IL.Emit(OpCodes.Conv_R8);
                if (!dropResult) context.IL.Emit(OpCodes.Dup);
                variable.EmitStore(context);
            }

            public void EmitConvert(IBlockContext context) => context.IL.Emit(OpCodes.Conv_R8);
        }

        internal class TO2Float : BuildinType {
            private OperatorCollection allowedPrefixOperators;
            private OperatorCollection allowedPostfixOperators;
            private Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private Dictionary<string, IFieldAccessFactory> allowedFields;
            private IAssignEmitter intToFloatAssign;

            internal TO2Float() {
                allowedPrefixOperators = new OperatorCollection {
                    {Operator.Neg, new DirectOperatorEmitter(() => BuildinType.Unit, () => BuildinType.Float, OpCodes.Neg)},
                    {Operator.Add, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Add)},
                    {Operator.Sub, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Sub)},
                    {Operator.Mul, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Mul)},
                    {Operator.Div, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Div)},
                };
                allowedPostfixOperators = new OperatorCollection {
                    {Operator.Add, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Add)},
                    {Operator.AddAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Add)},
                    {Operator.Sub, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Sub)},
                    {Operator.SubAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Sub)},
                    {Operator.Mul, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Mul)},
                    {Operator.MulAsign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Mul)},
                    {Operator.Div, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Div)},
                    {Operator.DivAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Div)},
                    {Operator.Eq, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Ceq)},
                    {Operator.NotEq, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Ceq, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.Gt, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Cgt)},
                    {Operator.Lt, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Clt)},
                    {Operator.Ge, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Clt, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.Le, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Bool, OpCodes.Cgt, OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {"to_string", new BoundMethodInvokeFactory("Convert the float to string.", () => BuildinType.String, () => new List<FunctionParameter>(), false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToString") )},
                    {"to_fixed", new BoundMethodInvokeFactory("Convert the float to string with fixed number of `decimals`.", () => BuildinType.String, () => new List<FunctionParameter>() { new FunctionParameter("decimals", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToFixed") )},
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"to_int", new InlineFieldAccessFactory("Value converted to int (will be truncated as necessary)", () => BuildinType.Int, OpCodes.Conv_I8)},
                    {"abs", new BoundPropertyLikeFieldAccessFactory("Absolute value", () => BuildinType.Float, typeof(Math), typeof(Math).GetMethod("Abs", new Type[] { typeof(double) }))},
                    {"sign", new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)", () => BuildinType.Int, typeof(Math),  typeof(Math).GetMethod("Sign", new Type[] { typeof(double) }), OpCodes.Conv_I8)},
                };
                intToFloatAssign = new IntToFloatAssign();
            }

            public override string Name => "float";
            public override Type GeneratedType(ModuleContext context) => typeof(double);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedPostfixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

            public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => otherType == BuildinType.Int || otherType == BuildinType.Float;

            public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => otherType == BuildinType.Int ? intToFloatAssign : DefaultAssignEmitter.Instance;
        }

        public static RealizedType Float = new TO2Float();

        internal class TO2SString : BuildinType {
            private OperatorCollection allowedOperators;
            private Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private Dictionary<string, IFieldAccessFactory> allowedFields;

            internal TO2SString() {
                allowedOperators = new OperatorCollection {
                    {Operator.Add, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.String, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }))},
                    {Operator.AddAssign, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.String, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }))},
                    {Operator.Eq, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(string) }))},
                    {Operator.NotEq, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(string) }), OpCodes.Ldc_I4_0, OpCodes.Ceq)},
                    {Operator.Gt, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }), OpCodes.Ldc_I4_0, OpCodes.Cgt)},
                    {Operator.Ge, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }), OpCodes.Ldc_I4_M1, OpCodes.Cgt)},
                    {Operator.Lt, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }), OpCodes.Ldc_I4_0, OpCodes.Clt)},
                    {Operator.Le, new StaticMethodOperatorEmitter(() => BuildinType.String, () => BuildinType.Bool, typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) }), OpCodes.Ldc_I4_1, OpCodes.Clt)},
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {"repeat", new BoundMethodInvokeFactory("Repeat the string `count` number of time", () => BuildinType.String, () => new List<FunctionParameter>() { new FunctionParameter("count", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringRepeat") )},
                    {"pad_left", new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the left side", () => BuildinType.String, () => new List<FunctionParameter>() { new FunctionParameter("length", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadLeft") )},
                    {"pad_right", new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the right side", () => BuildinType.String, () => new List<FunctionParameter>() { new FunctionParameter("length", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadRight") )},
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"length", new BoundPropertyLikeFieldAccessFactory("Length of the string, i.e. number of characters in the string", () => BuildinType.Int, typeof(String), typeof(String).GetProperty("Length").GetGetMethod(), OpCodes.Conv_I8)},
                };
            }

            public override string Name => "string";

            public override Type GeneratedType(ModuleContext context) => typeof(string);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }

        public static RealizedType String = new TO2SString();

        public static RealizedType Range = new RangeType();
    }
}
