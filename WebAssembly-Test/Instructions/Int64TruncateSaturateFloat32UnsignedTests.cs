using System;
using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64TruncateSaturateFloat32Unsigned"/> instruction.
    /// </summary>
    public class Int64TruncateSaturateFloat32UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64TruncateSaturateFloat32Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64TruncateSaturatedUnsignedFloat32_Compiled() {
            var exports = ConversionTestBase<float, long>.CreateInstance(new LocalGet(0),
                new Int64TruncateSaturateFloat32Unsigned(), new End());

            // Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/conversions.wast
            Assert.Equal(0, exports.Test(0.0f));
            Assert.Equal(0, exports.Test(-0.0f));
            Assert.Equal(0, exports.Test(float.Epsilon));
            Assert.Equal(0, exports.Test(-float.Epsilon));
            Assert.Equal(1, exports.Test(1.0f));
            Assert.Equal(1, exports.Test(1.5f));
            Assert.Equal(4294967296, exports.Test(4294967296f));
            Assert.Equal(-1099511627776, exports.Test(18446742974197923840.0f));
            Assert.Equal(unchecked((long)0xffffffffffffffff), exports.Test(18446744073709551616.0f));
            Assert.Equal(0x0000000000000000, exports.Test(-1.0f));
            Assert.Equal(unchecked((long)0xffffffffffffffff), exports.Test(float.PositiveInfinity));
            Assert.Equal(0x0000000000000000, exports.Test(float.NegativeInfinity));
            Assert.Equal(0, exports.Test(float.NaN));
            Assert.Equal(0, exports.Test(-float.NaN));
        }

    }
}
