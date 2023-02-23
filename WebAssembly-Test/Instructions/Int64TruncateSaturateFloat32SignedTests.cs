using System;
using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64TruncateSaturateFloat32Signed"/> instruction.
    /// </summary>
    public class Int64TruncateSaturateFloat32SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64TruncateSaturateFloat32Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64TruncateSaturatedSignedFloat32_Compiled() {
            var exports = ConversionTestBase<float, long>.CreateInstance(new LocalGet(0),
                new Int64TruncateSaturateFloat32Signed(), new End());

            // Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/conversions.wast
            Assert.Equal(0, exports.Test(0.0f));
            Assert.Equal(0, exports.Test(-0.0f));
            Assert.Equal(0, exports.Test(float.Epsilon));
            Assert.Equal(0, exports.Test(-float.Epsilon));
            Assert.Equal(1, exports.Test(1.0f));
            Assert.Equal(1, exports.Test(1.5f));
            Assert.Equal(-1, exports.Test(-1.0f));
            Assert.Equal(-1, exports.Test(-1.5f));
            Assert.Equal(-1, exports.Test(-1.9f));
            Assert.Equal(-2, exports.Test(-2.0f));
            Assert.Equal(4294967296, exports.Test(4294967296f));
            Assert.Equal(-4294967296, exports.Test(-4294967296f));
            Assert.Equal(9223371487098961920, exports.Test(9223371487098961920.0f));
            Assert.Equal(-9223372036854775808, exports.Test(-9223372036854775808.0f));
            Assert.Equal(0x7fffffffffffffff, exports.Test(9223372036854775808.0f));
            Assert.Equal(unchecked((long)0x8000000000000000), exports.Test(-9223373136366403584.0f));
            Assert.Equal(0x7fffffffffffffff, exports.Test(float.PositiveInfinity));
            Assert.Equal(unchecked((long)0x8000000000000000), exports.Test(float.NegativeInfinity));
            Assert.Equal(0, exports.Test(float.NaN));
            Assert.Equal(0, exports.Test(-float.NaN));
        }

    }
}
