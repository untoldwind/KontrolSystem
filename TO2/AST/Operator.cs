using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public enum Operator {
        Assign,       // =
        Add,          // +
        AddAssign,    // +=
        Sub,          // -
        SubAssign,    // -=
        Mul,          // *
        MulAssign,     // *=
        Div,          // /
        DivAssign,    // /=
        Mod,          // %
        BitOr,        // |
        BitOrAssign,  // |=
        BitAnd,       // &
        BitAndAssign, // &=
        BitXor,       // ^
        BitXorAssign, // ^=
        Eq,           // ==
        NotEq,        // !=
        Lt,           // <
        Le,           // <=
        Gt,           // >
        Ge,           // >=
        Neg,          // -
        Not,          // !
        BitNot,       // ~
        BoolAnd,      // &&
        BoolOr,       // ||
        Unwrap        // ?
    }

    public interface IOperatorEmitter {
        TO2Type ResultType {
            get;
        }
        TO2Type OtherType {
            get;
        }

        bool Accepts(ModuleContext context, TO2Type otherType);

        void EmitCode(IBlockContext context, Node target);

        void EmitAssign(IBlockContext context, IBlockVariable variable, Node target);
    }

    public class DirectOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherType;
        private readonly Func<TO2Type> resultType;
        private readonly OpCode[] opCodes;

        public DirectOperatorEmitter(Func<TO2Type> _otherType, Func<TO2Type> _resultType, params OpCode[] _opCodes) {
            otherType = _otherType;
            resultType = _resultType;
            opCodes = _opCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type _otherType) => otherType().IsAssignableFrom(context, _otherType);

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
    }

    public class StaticMethodOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherType;
        private readonly Func<TO2Type> resultType;
        private readonly MethodInfo methodInfo;
        private readonly OpCode[] postOpCodes;

        public StaticMethodOperatorEmitter(Func<TO2Type> _otherType, Func<TO2Type> _resultType, MethodInfo _methodInfo, params OpCode[] _postOpCodes) {
            otherType = _otherType;
            resultType = _resultType;
            methodInfo = _methodInfo;
            postOpCodes = _postOpCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type _otherType) => otherType().IsAssignableFrom(context, _otherType);

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
    }
}
