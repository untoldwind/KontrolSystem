using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Divide"/> instruction.
    /// </summary>
    public class Float32DivideTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Divide"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Divide_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Constant(3),
                new Float32Divide(), new End());

            Assert.Equal(0, exports.Test(0));
            Assert.Equal(3, exports.Test(9));
            Assert.Equal(-2, exports.Test(-6));
        }
    }
}
