using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Negate"/> instruction.
    /// </summary>
    public class Float64NegateTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Negate"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Negate_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Negate(), new End());

            foreach (var value in Samples.Double) Assert.Equal(-value, exports.Test(value));
        }
    }
}
