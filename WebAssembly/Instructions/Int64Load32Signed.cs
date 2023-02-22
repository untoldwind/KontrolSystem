﻿using System.Reflection.Emit;

namespace WebAssembly.Instructions {

    /// <summary>
    /// Load 4 bytes and sign-extend i32 to i64.
    /// </summary>
    public class Int64Load32Signed : MemoryReadInstruction {
        /// <summary>
        /// Always <see cref="OpCode.Int64Load32Signed"/>.
        /// </summary>
        public sealed override OpCode OpCode => OpCode.Int64Load32Signed;

        /// <summary>
        /// Creates a new  <see cref="Int64Load32Signed"/> instance.
        /// </summary>
        public Int64Load32Signed() {
        }

        internal Int64Load32Signed(Reader reader) : base(reader) {
        }

        private protected sealed override WebAssemblyValueType Type => WebAssemblyValueType.Int64;

        private protected sealed override byte Size => 4;

        private protected sealed override System.Reflection.Emit.OpCode EmittedOpCode => OpCodes.Ldind_I4;

        private protected sealed override System.Reflection.Emit.OpCode ConversionOpCode => OpCodes.Conv_I8;
    }
}
