using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32ConvertInt32Signed"/> instruction.
    /// </summary>
    public class Float32ConvertInt32SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32ConvertInt32Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Float32ConvertSignedInt32_Compiled() {
            var exports = ConversionTestBase<int, float>.CreateInstance(
                new LocalGet(0), new Float32ConvertInt32Signed(), new End());

            foreach (var value in Samples.Int32) Assert.Equal(value, exports.Test(value));
        }
    }
}
