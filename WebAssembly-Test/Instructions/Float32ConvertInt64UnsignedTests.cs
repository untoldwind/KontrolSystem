using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32ConvertInt64Unsigned"/> instruction.
    /// </summary>
    public class Float32ConvertInt64UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32ConvertInt64Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Float32ConvertUnsignedInt64_Compiled() {
            var exports = ConversionTestBase<long, float>.CreateInstance(
                new LocalGet(0), new Float32ConvertInt64Unsigned(), new End());

            foreach (var value in Samples.UInt64) Assert.Equal(value, exports.Test((long)value));
        }
    }
}
