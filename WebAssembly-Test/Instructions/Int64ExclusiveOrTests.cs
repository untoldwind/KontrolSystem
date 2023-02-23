﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64ExclusiveOr"/> instruction.
    /// </summary>
    public class Int64ExclusiveOrTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64ExclusiveOr"/> instruction.
        /// </summary>
        [Fact]
        public void Int64ExclusiveOr_Compiled() {
            const int or = 0xF;

            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64Constant(or),
                new Int64ExclusiveOr(), new End());

            foreach (var value in new long[] { 0x00, 0x0F, 0xF0, 0xFF, }) Assert.Equal(value ^ or, exports.Test(value));
        }
    }
}
