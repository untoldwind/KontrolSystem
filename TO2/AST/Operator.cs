using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public enum Operator {
        Assign, // =
        Add, // +
        AddAssign, // +=
        Sub, // -
        SubAssign, // -=
        Mul, // *
        MulAssign, // *=
        Div, // /
        DivAssign, // /=
        Mod, // %
        BitOr, // |
        BitOrAssign, // |=
        BitAnd, // &
        BitAndAssign, // &=
        BitXor, // ^
        BitXorAssign, // ^=
        Eq, // ==
        NotEq, // !=
        Lt, // <
        Le, // <=
        Gt, // >
        Ge, // >=
        Neg, // -
        Not, // !
        BitNot, // ~
        BoolAnd, // &&
        BoolOr, // ||
        Unwrap // ?
    }

    public interface IOperatorEmitter {
        TO2Type ResultType { get; }
        TO2Type OtherType { get; }

        bool Accepts(ModuleContext context, TO2Type otherType);

        void EmitCode(IBlockContext context, Node target);

        void EmitAssign(IBlockContext context, IBlockVariable variable, Node target);

        IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
    }

    public class DirectOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherType;
        private readonly Func<TO2Type> resultType;
        private readonly OpCode[] opCodes;

        public DirectOperatorEmitter(Func<TO2Type> otherType, Func<TO2Type> resultType, params OpCode[] opCodes) {
            this.otherType = otherType;
            this.resultType = resultType;
            this.opCodes = opCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            this.otherType().IsAssignableFrom(context, otherType);

        public TO2Type OtherType => otherType();

        public TO2Type ResultType => resultType();

        public void EmitCode(IBlockContext context, Node target) {
            foreach (OpCode opCode in opCodes) context.IL.Emit(opCode);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;
    }

    public class StaticMethodOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherType;
        private readonly Func<TO2Type> resultType;
        private readonly MethodInfo methodInfo;
        private readonly OpCode[] postOpCodes;

        public StaticMethodOperatorEmitter(Func<TO2Type> otherType, Func<TO2Type> resultType, MethodInfo methodInfo,
            params OpCode[] postOpCodes) {
            this.otherType = otherType;
            this.resultType = resultType;
            this.methodInfo = methodInfo;
            this.postOpCodes = postOpCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            this.otherType().IsAssignableFrom(context, otherType);

        public TO2Type OtherType => otherType();

        public TO2Type ResultType => resultType();

        public void EmitCode(IBlockContext context, Node target) {
            context.IL.EmitCall(OpCodes.Call, methodInfo, methodInfo.GetParameters().Length);
            foreach (OpCode opCOde in postOpCodes) context.IL.Emit(opCOde);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (methodInfo.IsGenericMethod) {
                Type[] arguments = methodInfo.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();

                return new StaticMethodOperatorEmitter(
                    () => otherType().UnderlyingType(context).FillGenerics(context, typeArguments),
                    () => resultType().UnderlyingType(context).FillGenerics(context, typeArguments),
                    methodInfo.MakeGenericMethod(arguments), postOpCodes);
            }

            return this;
        }
    }
}
