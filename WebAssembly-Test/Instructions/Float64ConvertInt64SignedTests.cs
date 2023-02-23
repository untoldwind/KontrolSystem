using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64ConvertInt64Signed"/> instruction.
    /// </summary>
    public class Float64ConvertInt64SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64ConvertInt64Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Float64ConvertSignedInt64_Compiled() {
            var exports = ConversionTestBase<long, double>.CreateInstance(
                new LocalGet(0), new Float64ConvertInt64Signed(), new End());

            foreach (var value in Samples.Int64) Assert.Equal(value, exports.Test(value));
        }
    }
}
