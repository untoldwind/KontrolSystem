using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Multiply"/> instruction.
    /// </summary>
    public class Float32MultiplyTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Multiply"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Multiply_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Constant(3),
                new Float32Multiply(), new End());

            Assert.Equal(0, exports.Test(0));
            Assert.Equal(9, exports.Test(3));
            Assert.Equal(-6, exports.Test(-2));
        }
    }
}
