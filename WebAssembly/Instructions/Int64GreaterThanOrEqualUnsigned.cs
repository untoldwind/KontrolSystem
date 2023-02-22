﻿namespace WebAssembly.Instructions {

    /// <summary>
    /// Unsigned greater than or equal.
    /// </summary>
    public class Int64GreaterThanOrEqualUnsigned : ValueTwoToInt32NotEqualZeroInstruction {
        /// <summary>
        /// Always <see cref="OpCode.Int64GreaterThanOrEqualUnsigned"/>.
        /// </summary>
        public sealed override OpCode OpCode => OpCode.Int64GreaterThanOrEqualUnsigned;

        private protected sealed override WebAssemblyValueType ValueType => WebAssemblyValueType.Int64;

        private protected sealed override System.Reflection.Emit.OpCode EmittedOpCode =>
            System.Reflection.Emit.OpCodes.Clt_Un; //The result is compared for equality to zero, reversing it.

        /// <summary>
        /// Creates a new  <see cref="Int64GreaterThanOrEqualUnsigned"/> instance.
        /// </summary>
        public Int64GreaterThanOrEqualUnsigned() {
        }
    }
}
