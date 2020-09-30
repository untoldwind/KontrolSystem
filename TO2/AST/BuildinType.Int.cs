using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuildinType : RealizedType {
        private class TO2Int : BuildinType {
            private readonly OperatorCollection allowedPrefixOperators;
            private readonly OperatorCollection allowedSuffixOperators;
            private readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
            private readonly Dictionary<string, IFieldAccessFactory> allowedFields;

            internal TO2Int() {
                allowedPrefixOperators = new OperatorCollection {
                    {
                        Operator.Neg,
                        new DirectOperatorEmitter(() => BuildinType.Unit, () => BuildinType.Int, OpCodes.Neg)
                    },
                };
                allowedSuffixOperators = new OperatorCollection {
                    {
                        Operator.Add,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Add)
                    }, {
                        Operator.AddAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Add)
                    }, {
                        Operator.Sub,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Sub)
                    }, {
                        Operator.SubAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Sub)
                    }, {
                        Operator.Mul,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Mul)
                    }, {
                        Operator.MulAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Mul)
                    }, {
                        Operator.Div,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Div)
                    }, {
                        Operator.DivAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Div)
                    }, {
                        Operator.Mod,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Rem)
                    }, {
                        Operator.BitOr,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Or)
                    }, {
                        Operator.BitOrAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Or)
                    }, {
                        Operator.BitAnd,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.And)
                    }, {
                        Operator.BitAndAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.And)
                    }, {
                        Operator.BitXor,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Xor)
                    }, {
                        Operator.BitXorAssign,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Int, OpCodes.Xor)
                    }, {
                        Operator.Eq,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Ceq)
                    }, {
                        Operator.NotEq,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Ceq,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Gt,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Cgt)
                    }, {
                        Operator.Lt,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Clt)
                    }, {
                        Operator.Ge,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Clt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Le,
                        new DirectOperatorEmitter(() => BuildinType.Int, () => BuildinType.Bool, OpCodes.Cgt,
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    },
                };
                allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "to_string",
                        new BoundMethodInvokeFactory("Convert integer to string", () => BuildinType.String,
                            () => new List<RealizedParameter>(), false, typeof(FormatUtils),
                            typeof(FormatUtils).GetMethod("IntToString"))
                    }
                };
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "to_bool",
                        new InlineFieldAccessFactory("Value converted to bool (0 -> false, != 0 -> true)",
                            () => BuildinType.Bool, OpCodes.Conv_I4)
                    }, {
                        "to_float",
                        new InlineFieldAccessFactory("Value converted to float", () => BuildinType.Float,
                            OpCodes.Conv_R8)
                    }, {
                        "abs",
                        new BoundPropertyLikeFieldAccessFactory("Absolute value", () => BuildinType.Int, typeof(Math),
                            typeof(Math).GetMethod("Abs", new Type[] {typeof(long)}))
                    }, {
                        "sign",
                        new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)",
                            () => BuildinType.Int, typeof(Math),
                            typeof(Math).GetMethod("Sign", new Type[] {typeof(long)}), OpCodes.Conv_I8)
                    },
                };
            }

            public override string Name => "int";
            public override Type GeneratedType(ModuleContext context) => typeof(long);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

            public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;
        }

        internal class IntToFloatAssign : IAssignEmitter {
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
