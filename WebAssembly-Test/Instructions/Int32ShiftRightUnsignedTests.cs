﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32ShiftRightUnsigned"/> instruction.
    /// </summary>
    public class Int32ShiftRightUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32ShiftRightUnsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32ShiftRightUnsigned_Compiled() {
            const int amount = 0xF;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(amount),
                new Int32ShiftRightUnsigned(), new End());

            foreach (var value in new uint[] { 0x00, 0x01, 0x02, 0x0F, 0xF0, 0xFF, })
                Assert.Equal(value >> amount, (uint)exports.Test((int)value));
        }
    }
}
