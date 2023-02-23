using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32TruncateFloat64Signed"/> instruction.
    /// </summary>
    public class Int32TruncateFloat64SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32TruncateFloat64Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int32TruncateSignedFloat64_Compiled() {
            var exports = ConversionTestBase<double, int>.CreateInstance(
                new LocalGet(0), new Int32TruncateFloat64Signed(), new End());

            foreach (var value in new[] { 0, 1.5, -1.5 }) Assert.Equal((int)value, exports.Test(value));

            const double exceptional = 123445678901234.0;
            Assert.Throws<System.OverflowException>(() => exports.Test(exceptional));
        }
    }
}
