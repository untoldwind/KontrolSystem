using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64PromoteFloat32"/> instruction.
    /// </summary>
    public class Float64PromoteFloat32Tests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64PromoteFloat32"/> instruction.
        /// </summary>
        [Fact]
        public void Float64PromoteFloat32_Compiled() {
            var exports = ConversionTestBase<float, double>.CreateInstance(
                new LocalGet(0), new Float64PromoteFloat32(), new End());

            foreach (var value in Samples.Single) Assert.Equal(value, exports.Test(value));
        }
    }
}
