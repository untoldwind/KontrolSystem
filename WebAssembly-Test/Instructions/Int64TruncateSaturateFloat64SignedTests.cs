using System;
using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64TruncateSaturateFloat64Signed"/> instruction.
    /// </summary>
    public class Int64TruncateSaturateFloat64SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64TruncateSaturateFloat64Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64TruncateSaturatedSignedFloat64_Compiled() {
            var exports = ConversionTestBase<double, long>.CreateInstance(new LocalGet(0),
                new Int64TruncateSaturateFloat64Signed(), new End());

            // Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/conversions.wast
            Assert.Equal(0, exports.Test(0.0));
            Assert.Equal(0, exports.Test(-0.0));
            Assert.Equal(0, exports.Test(double.Epsilon));
            Assert.Equal(0, exports.Test(-double.Epsilon));
            Assert.Equal(1, exports.Test(1.0));
            Assert.Equal(1, exports.Test(BitConverter.Int64BitsToDouble(0x3ff199999999999a)));
            Assert.Equal(1, exports.Test(1.5));
            Assert.Equal(-1, exports.Test(-1.0));
            Assert.Equal(-1, exports.Test(BitConverter.Int64BitsToDouble(unchecked((long)0xbff199999999999a))));
            Assert.Equal(-1, exports.Test(-1.5));
            Assert.Equal(-1, exports.Test(-1.9));
            Assert.Equal(-2, exports.Test(-2.0));
            Assert.Equal(4294967296, exports.Test(4294967296));
            Assert.Equal(-4294967296, exports.Test(-4294967296));
            Assert.Equal(9223372036854774784, exports.Test(9223372036854774784.0));
            Assert.Equal(-9223372036854775808, exports.Test(-9223372036854775808.0));
            Assert.Equal(0x7fffffffffffffff, exports.Test(9223372036854775808.0));
            Assert.Equal(unchecked((long)0x8000000000000000), exports.Test(-9223372036854777856.0));
            Assert.Equal(0x7fffffffffffffff, exports.Test(double.PositiveInfinity));
            Assert.Equal(unchecked((long)0x8000000000000000), exports.Test(double.NegativeInfinity));
            Assert.Equal(0, exports.Test(double.NaN));
            Assert.Equal(0, exports.Test(AddPayload(double.NaN, 0x4000000000000)));
            Assert.Equal(0, exports.Test(-double.NaN));
            Assert.Equal(0, exports.Test(AddPayload(-double.NaN, 0x4000000000000)));
        }

        private static double AddPayload(double doubleValue, long payload) {
            var doubleValueAsInt = BitConverter.DoubleToInt64Bits(doubleValue);
            doubleValueAsInt |= payload;
            return BitConverter.Int64BitsToDouble(doubleValueAsInt);
        }
    }
}
