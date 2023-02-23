using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Subtract"/> instruction.
    /// </summary>
    public class Float32SubtractTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Subtract"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Subtract_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Constant(1),
                new Float32Subtract(), new End());

            Assert.Equal(-1, exports.Test(0));
            Assert.Equal(4, exports.Test(5));
        }
    }
}
