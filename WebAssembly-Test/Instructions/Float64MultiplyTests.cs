using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Multiply"/> instruction.
    /// </summary>
    public class Float64MultiplyTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Multiply"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Multiply_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Constant(3),
                new Float64Multiply(), new End());

            Assert.Equal(0, exports.Test(0));
            Assert.Equal(9, exports.Test(3));
            Assert.Equal(-6, exports.Test(-2));
        }
    }
}
