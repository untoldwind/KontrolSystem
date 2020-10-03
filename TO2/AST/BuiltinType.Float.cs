using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType {
        private class TO2Float : BuiltinType {
            private readonly OperatorCollection allowedPrefixOperators;
            private readonly OperatorCollection allowedSuffixOperators;
            private readonly IAssignEmitter intToFloatAssign;
            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
            public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

            internal TO2Float() {
                allowedPrefixOperators = new OperatorCollection {
                    {
                        Operator.Neg,
                        new DirectOperatorEmitter(() => BuiltinType.Unit, () => BuiltinType.Float, OpCodes.Neg)
                    }, {
                        Operator.Add,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Add)
                    }, {
                        Operator.Sub,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Sub)
                    }, {
                        Operator.Mul,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Mul)
                    }, {
                        Operator.Div,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Div)
                    },
                };
                allowedSuffixOperators = new OperatorCollection {
                    {
                        Operator.Add,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Add)
                    }, {
                        Operator.AddAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Add)
                    }, {
                        Operator.Sub,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Sub)
                    }, {
                        Operator.SubAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Sub)
                    }, {
                        Operator.Mul,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Mul)
                    }, {
                        Operator.MulAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Mul)
                    }, {
                        Operator.Div,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Div)
                    }, {
                        Operator.DivAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Div)
                    }, {
                        Operator.Mod,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Rem)
                    }, {
                        Operator.ModAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Float, OpCodes.Rem)
                    }, {
                        Operator.Eq,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Ceq)
                    }, {
                        Operator.NotEq,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Ceq,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Gt,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Cgt)
                    }, {
                        Operator.Lt,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Clt)
                    }, {
                        Operator.Ge,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Clt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Le,
                        new DirectOperatorEmitter(() => BuiltinType.Float, () => BuiltinType.Bool, OpCodes.Cgt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    },
                };
                DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "to_string",
                        new BoundMethodInvokeFactory("Convert the float to string.", () => BuiltinType.String,
                            () => new List<RealizedParameter>(), false, typeof(FormatUtils),
                            typeof(FormatUtils).GetMethod("FloatToString"))
                    }, {
                        "to_fixed",
                        new BoundMethodInvokeFactory("Convert the float to string with fixed number of `decimals`.",
                            () => BuiltinType.String,
                            () => new List<RealizedParameter>() {new RealizedParameter("decimals", BuiltinType.Int)},
                            false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToFixed"))
                    },
                };
                DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "to_int",
                        new InlineFieldAccessFactory("Value converted to int (will be truncated as necessary)",
                            () => BuiltinType.Int, OpCodes.Conv_I8)
                    }, {
                        "abs",
                        new BoundPropertyLikeFieldAccessFactory("Absolute value", () => BuiltinType.Float, typeof(Math),
                            typeof(Math).GetMethod("Abs", new[] {typeof(double)}))
                    }, {
                        "sign",
                        new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)",
                            () => BuiltinType.Int, typeof(Math),
                            typeof(Math).GetMethod("Sign", new[] {typeof(double)}), OpCodes.Conv_I8)
                    },
                };
                intToFloatAssign = new IntToFloatAssign();
            }

            public override string Name => "float";
            public override Type GeneratedType(ModuleContext context) => typeof(double);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;


            public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) =>
                otherType == BuiltinType.Int || otherType == BuiltinType.Float;

            public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) =>
                otherType == BuiltinType.Int ? intToFloatAssign : DefaultAssignEmitter.Instance;
        }
    }
}
