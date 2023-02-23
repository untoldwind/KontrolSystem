﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32EqualZero"/> instruction.
    /// </summary>
    public class Int32EqualZeroTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32EqualZero"/> instruction.
        /// </summary>
        [Fact]
        public void Int32EqualZero_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32EqualZero(), new End());

            foreach (var value in Samples.Int32) Assert.Equal(value == 0, exports.Test(value) != 0);
        }
    }
}
