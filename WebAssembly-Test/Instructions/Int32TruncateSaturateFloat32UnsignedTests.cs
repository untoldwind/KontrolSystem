using System;
using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32TruncateSaturateFloat32Unsigned"/> instruction.
    /// </summary>
    public class Int32TruncateSaturateFloat32UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32TruncateSaturateFloat32Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32TruncateSaturatedUnsignedFloat32_Compiled() {
            var exports = ConversionTestBase<float, int>.CreateInstance(new LocalGet(0),
                new Int32TruncateSaturateFloat32Unsigned(), new End());

            // Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/conversions.wast
            Assert.Equal(0, exports.Test(0.0f));
            Assert.Equal(0, exports.Test(-0.0f));
            Assert.Equal(0, exports.Test(float.Epsilon));
            Assert.Equal(0, exports.Test(-float.Epsilon));
            Assert.Equal(1, exports.Test(1.0f));
            Assert.Equal(1, exports.Test(1.5f));
            Assert.Equal(1, exports.Test(1.9f));
            Assert.Equal(2, exports.Test(2.0f));
            Assert.Equal(-2147483648, exports.Test(2147483648f));
            Assert.Equal(-256, exports.Test(4294967040.0f));
            Assert.Equal(unchecked((int)0xffffffff), exports.Test(4294967296.0f));
            Assert.Equal(0x00000000, exports.Test(-1.0f));
            Assert.Equal(unchecked((int)0xffffffff), exports.Test(float.PositiveInfinity));
            Assert.Equal(0x00000000, exports.Test(float.NegativeInfinity));
            Assert.Equal(0, exports.Test(float.NaN));
            Assert.Equal(0, exports.Test(-float.NaN));
        }
    }
}
