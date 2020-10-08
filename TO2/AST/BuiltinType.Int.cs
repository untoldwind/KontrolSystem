using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType : RealizedType {
        private class TO2Int : BuiltinType {
            private readonly OperatorCollection allowedPrefixOperators;
            private readonly OperatorCollection allowedSuffixOperators;
            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
            public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

            internal TO2Int() {
                allowedPrefixOperators = new OperatorCollection {
                    {
                        Operator.Neg,
                        new DirectOperatorEmitter(() => BuiltinType.Unit, () => BuiltinType.Int, OpCodes.Neg)
                    },
                };
                allowedSuffixOperators = new OperatorCollection {
                    {
                        Operator.Add,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Add)
                    }, {
                        Operator.AddAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Add)
                    }, {
                        Operator.Sub,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Sub)
                    }, {
                        Operator.SubAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Sub)
                    }, {
                        Operator.Mul,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Mul)
                    }, {
                        Operator.MulAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Mul)
                    }, {
                        Operator.Div,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Div)
                    }, {
                        Operator.DivAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Div)
                    }, {
                        Operator.Mod,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Rem)
                    }, {
                        Operator.ModAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Rem)
                    }, {
                        Operator.BitOr,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Or)
                    }, {
                        Operator.BitOrAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Or)
                    }, {
                        Operator.BitAnd,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.And)
                    }, {
                        Operator.BitAndAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.And)
                    }, {
                        Operator.BitXor,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Xor)
                    }, {
                        Operator.BitXorAssign,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Int, OpCodes.Xor)
                    }, {
                        Operator.Eq,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Ceq)
                    }, {
                        Operator.NotEq,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Ceq,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Gt,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Cgt)
                    }, {
                        Operator.Lt,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Clt)
                    }, {
                        Operator.Ge,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Clt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Le,
                        new DirectOperatorEmitter(() => BuiltinType.Int, () => BuiltinType.Bool, OpCodes.Cgt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    },
                };
                DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "to_string",
                        new BoundMethodInvokeFactory("Convert integer to string", () => BuiltinType.String,
                            () => new List<RealizedParameter>(), false, typeof(FormatUtils),
                            typeof(FormatUtils).GetMethod("IntToString"))
                    }
                };
                DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "to_bool",
                        new InlineFieldAccessFactory("Value converted to bool (0 -> false, != 0 -> true)",
                            () => BuiltinType.Bool, OpCodes.Conv_I4)
                    }, {
                        "to_float",
                        new InlineFieldAccessFactory("Value converted to float", () => BuiltinType.Float,
                            OpCodes.Conv_R8)
                    }, {
                        "abs",
                        new BoundPropertyLikeFieldAccessFactory("Absolute value", () => BuiltinType.Int, typeof(Math),
                            typeof(Math).GetMethod("Abs", new[] {typeof(long)}), null)
                    }, {
                        "sign",
                        new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)",
                            () => BuiltinType.Int, typeof(Math),
                            typeof(Math).GetMethod("Sign", new[] {typeof(long)}), null, OpCodes.Conv_I8)
                    },
                };
            }

            public override string Name => "int";
            public override Type GeneratedType(ModuleContext context) => typeof(long);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;
        }

        private class IntToFloatAssign : IAssignEmitter {
            public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
                bool dropResult) {
                expression.EmitCode(context, false);
                context.IL.Emit(OpCodes.Conv_R8);
                if (!dropResult) context.IL.Emit(OpCodes.Dup);
                variable.EmitStore(context);
            }

            public void EmitConvert(IBlockContext context) => context.IL.Emit(OpCodes.Conv_R8);
        }
    }
}
