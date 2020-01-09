using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuildinType : RealizedType {
        internal class TO2Float : BuildinType {
            private OperatorCollection allowedPrefixOperators;
            private OperatorCollection allowedSuffixOperators;
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
                allowedSuffixOperators = new OperatorCollection {
                    {Operator.Add, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Add)},
                    {Operator.AddAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Add)},
                    {Operator.Sub, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Sub)},
                    {Operator.SubAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Sub)},
                    {Operator.Mul, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Mul)},
                    {Operator.MulAssign, new DirectOperatorEmitter(() => BuildinType.Float, () => BuildinType.Float, OpCodes.Mul)},
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
                    {"to_string", new BoundMethodInvokeFactory("Convert the float to string.", () => BuildinType.String, () => new List<RealizedParameter>(), false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToString") )},
                    {"to_fixed", new BoundMethodInvokeFactory("Convert the float to string with fixed number of `decimals`.", () => BuildinType.String, () => new List<RealizedParameter>() { new RealizedParameter("decimals", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToFixed") )},
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

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

            public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => otherType == BuildinType.Int || otherType == BuildinType.Float;

            public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => otherType == BuildinType.Int ? intToFloatAssign : DefaultAssignEmitter.Instance;
        }
    }
}
