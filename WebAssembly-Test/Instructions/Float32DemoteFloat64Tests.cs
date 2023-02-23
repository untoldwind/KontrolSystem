using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32DemoteFloat64"/> instruction.
    /// </summary>
    public class Float32DemoteFloat64Tests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32DemoteFloat64"/> instruction.
        /// </summary>
        [Fact]
        public void Float32DemoteFloat64_Compiled() {
            var exports = ConversionTestBase<double, float>.CreateInstance(
                new LocalGet(0), new Float32DemoteFloat64(), new End());

            foreach (var value in Samples.Double) Assert.Equal((float)value, exports.Test(value));
        }
    }
}
