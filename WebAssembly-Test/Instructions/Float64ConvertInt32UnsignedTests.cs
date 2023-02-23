using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64ConvertInt32Unsigned"/> instruction.
    /// </summary>
    public class Float64ConvertInt32UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64ConvertInt32Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Float64ConvertUnsignedInt32_Compiled() {
            var exports = ConversionTestBase<int, double>.CreateInstance(
                new LocalGet(0), new Float64ConvertInt32Unsigned(), new End());

            foreach (var value in Samples.UInt32) Assert.Equal(value, exports.Test((int)value));
        }
    }
}
