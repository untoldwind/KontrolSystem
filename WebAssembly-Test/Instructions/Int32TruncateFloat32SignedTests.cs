﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32TruncateFloat32Signed"/> instruction.
    /// </summary>
    public class Int32TruncateFloat32SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32TruncateFloat32Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int32TruncateSignedFloat32_Compiled() {
            var exports = ConversionTestBase<float, int>.CreateInstance(
                new LocalGet(0), new Int32TruncateFloat32Signed(), new End());

            foreach (var value in new[] { 0, 1.5f, -1.5f }) Assert.Equal((int)value, exports.Test(value));

            const float exceptional = 123445678901234f;
            Assert.Throws<System.OverflowException>(() => exports.Test(exceptional));
        }
    }
}
