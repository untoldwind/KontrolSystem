using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64ConvertInt64Unsigned"/> instruction.
    /// </summary>
    public class Float64ConvertInt64UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64ConvertInt64Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Float64ConvertUnsignedInt64_Compiled() {
            var exports = ConversionTestBase<long, double>.CreateInstance(
                new LocalGet(0), new Float64ConvertInt64Unsigned(), new End());

            foreach (var value in Samples.UInt64) Assert.Equal(value, exports.Test((long)value));
        }
    }
}
