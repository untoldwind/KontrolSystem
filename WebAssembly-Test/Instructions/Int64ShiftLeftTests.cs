﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64ShiftLeft"/> instruction.
    /// </summary>
    public class Int64ShiftLeftTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64ShiftLeft"/> instruction.
        /// </summary>
        [Fact]
        public void Int64ShiftLeft_Compiled() {
            const int amount = 0xF;

            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64Constant(amount),
                new Int64ShiftLeft(), new End());

            foreach (var value in new long[] { 0x00, 0x01, 0x02, 0x0F, 0xF0, 0xFF, })
                Assert.Equal(value << amount, exports.Test(value));
        }
    }
}
