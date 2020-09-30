using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType : RealizedType {
        private class TO2Bool : BuiltinType {
            private readonly OperatorCollection allowedPrefixOperators;
            private readonly OperatorCollection allowedSuffixOperators;
            private readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private readonly Dictionary<string, IFieldAccessFactory> allowedFields;

            internal TO2Bool() {
                allowedPrefixOperators = new OperatorCollection {
                    {
                        Operator.Not,
                        new DirectOperatorEmitter(() => BuiltinType.Unit, () => BuiltinType.Bool, OpCodes.Ldc_I4_0,
                            OpCodes.Ceq)
                    }
                };
                allowedSuffixOperators = new OperatorCollection {
                    {
                        Operator.Eq,
                        new DirectOperatorEmitter(() => BuiltinType.Bool, () => BuiltinType.Bool, OpCodes.Ceq)
                    }, {
                        Operator.NotEq,
                        new DirectOperatorEmitter(() => BuiltinType.Bool, () => BuiltinType.Bool, OpCodes.Ldc_I4_0,
                            OpCodes.Ceq)
                    }, {
                        Operator.BoolAnd,
                        new DirectOperatorEmitter(() => BuiltinType.Bool, () => BuiltinType.Bool, OpCodes.And)
                    }, {
                        Operator.BoolOr,
                        new DirectOperatorEmitter(() => BuiltinType.Bool, () => BuiltinType.Bool, OpCodes.Or)
                    }
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "to_string",
                        new BoundMethodInvokeFactory("Convert boolean to string", () => BuiltinType.String,
                            () => new List<RealizedParameter>(), false, typeof(FormatUtils),
                            typeof(FormatUtils).GetMethod("BoolToString"))
                    }
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "to_int",
                        new InlineFieldAccessFactory("Value converted to integer (false -> 0, true -> 1)",
                            () => BuiltinType.Int, OpCodes.Conv_I8)
                    }, {
                        "to_float",
                        new InlineFieldAccessFactory("Value converted to float (flase -> 0.0, true -> 1.0)",
                            () => BuiltinType.Float, OpCodes.Conv_R8)
                    },
                };
            }

            public override string Name => "bool";
            public override Type GeneratedType(ModuleContext context) => typeof(bool);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }
    }
}
