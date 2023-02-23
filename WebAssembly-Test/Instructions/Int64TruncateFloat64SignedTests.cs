using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64TruncateFloat64Signed"/> instruction.
    /// </summary>
    public class Int64TruncateFloat64SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64TruncateFloat64Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64TruncateSignedFloat64_Compiled() {
            var exports = ConversionTestBase<double, long>.CreateInstance(
                new LocalGet(0), new Int64TruncateFloat64Signed(), new End());

            foreach (var value in new[] { 0, 1.5, -1.5, 123445678901234.0 })
                Assert.Equal((long)value, exports.Test(value));

            const double exceptional = 1234456789012345678901234567890.0;
            Assert.Throws<System.OverflowException>(() => exports.Test(exceptional));
        }
    }
}
