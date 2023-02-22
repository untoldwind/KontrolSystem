﻿using System.Reflection.Emit;
using WebAssembly.Runtime.Compilation;

namespace WebAssembly.Instructions {

    /// <summary>
    /// Convert a signed 64-bit integer to a 64-bit float.
    /// </summary>
    public class Float64ConvertInt64Signed : SimpleInstruction {
        /// <summary>
        /// Always <see cref="OpCode.Float64ConvertInt64Signed"/>.
        /// </summary>
        public sealed override OpCode OpCode => OpCode.Float64ConvertInt64Signed;

        /// <summary>
        /// Creates a new  <see cref="Float64ConvertInt64Signed"/> instance.
        /// </summary>
        public Float64ConvertInt64Signed() {
        }

        internal sealed override void Compile(CompilationContext context) {
            var stack = context.Stack;

            context.PopStackNoReturn(OpCode.Float64ConvertInt64Signed, WebAssemblyValueType.Int64);

            context.Emit(OpCodes.Conv_R8);

            stack.Push(WebAssemblyValueType.Float64);
        }
    }
}
