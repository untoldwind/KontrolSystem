using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuildinType : RealizedType {
        private class TO2SString : BuildinType {
            private readonly OperatorCollection allowedOperators;
            private readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private readonly Dictionary<string, IFieldAccessFactory> allowedFields;

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
                    {"repeat", new BoundMethodInvokeFactory("Repeat the string `count` number of time", () => BuildinType.String, () => new List<RealizedParameter>() { new RealizedParameter("count", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringRepeat") )},
                    {"pad_left", new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the left side", () => BuildinType.String, () => new List<RealizedParameter>() { new RealizedParameter("length", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadLeft") )},
                    {"pad_right", new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the right side", () => BuildinType.String, () => new List<RealizedParameter>() { new RealizedParameter("length", BuildinType.Int) }, false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadRight") )},
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"length", new BoundPropertyLikeFieldAccessFactory("Length of the string, i.e. number of characters in the string", () => BuildinType.Int, typeof(String), typeof(String).GetProperty("Length").GetGetMethod(), OpCodes.Conv_I8)},
                };
            }

            public override string Name => "string";

            public override Type GeneratedType(ModuleContext context) => typeof(string);

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }
    }
}
