using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.AST {
    public class LiteralBool : Expression {
        public readonly bool value;

        public LiteralBool(bool _value, Position start = new Position(), Position end = new Position()) : base(start, end) => value = _value;

        public override void SetVariableContainer(IVariableContainer container) { }

        public override void Prepare(IBlockContext context) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Bool;

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!dropResult) context.IL.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
        }
    }

    public class LiteralString : Expression {
        public readonly string value;

        public LiteralString(string _value, Position start = new Position(), Position end = new Position()) : base(start, end) => value = _value;

        public LiteralString(char[] chars, Position start = new Position(), Position end = new Position()) : base(start, end) => value = new string(chars);

        public override void SetVariableContainer(IVariableContainer container) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.String;

        public override void Prepare(IBlockContext context) { }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!dropResult) context.IL.Emit(OpCodes.Ldstr, value);
        }
    }

    public class LiteralInt : Expression {
        public readonly long value;

        public LiteralInt(long _value, Position start = new Position(), Position end = new Position()) : base(start, end) => value = _value;

        public override void SetVariableContainer(IVariableContainer container) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Int;

        public override void Prepare(IBlockContext context) { }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!dropResult) context.IL.Emit(OpCodes.Ldc_I8, value);
        }
    }

    public class LiteralFloat : Expression {
        public readonly double value;

        public LiteralFloat(double _value, Position start = new Position(), Position end = new Position()) : base(start, end) => value = _value;

        public override void SetVariableContainer(IVariableContainer container) { }

        public override void Prepare(IBlockContext context) { }

        public override TO2Type ResultType(IBlockContext context) => BuildinType.Float;

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!dropResult) context.IL.Emit(OpCodes.Ldc_R8, value);
        }
    }
}
