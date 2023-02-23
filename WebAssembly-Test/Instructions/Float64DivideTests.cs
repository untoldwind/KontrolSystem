using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Divide"/> instruction.
    /// </summary>
    public class Float64DivideTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Divide"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Divide_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Constant(3),
                new Float64Divide(), new End());

            Assert.Equal(0, exports.Test(0));
            Assert.Equal(3, exports.Test(9));
            Assert.Equal(-2, exports.Test(-6));
        }
    }
}
