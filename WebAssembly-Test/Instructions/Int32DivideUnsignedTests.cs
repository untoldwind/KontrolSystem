﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32DivideUnsigned"/> instruction.
    /// </summary>
    public class Int32DivideUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Add"/> instruction.
        /// </summary>
        [Fact]
        public void Int32DivideUnsigned_Compiled() {
            const uint divisor = 2;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(divisor),
                new Int32DivideUnsigned(), new End());

            foreach (var value in new uint[] { 0, 1, 2, 3, 4, 5, })
                Assert.Equal(value / divisor, (uint)exports.Test((int)value));
        }
    }
}