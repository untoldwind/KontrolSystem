using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Negate"/> instruction.
    /// </summary>
    public class Float32NegateTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Negate"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Negate_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Negate(), new End());

            foreach (var value in Samples.Single) Assert.Equal(-value, exports.Test(value));
        }
    }
}
