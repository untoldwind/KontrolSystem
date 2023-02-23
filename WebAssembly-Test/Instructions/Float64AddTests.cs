using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Add"/> instruction.
    /// </summary>
    public class Float64AddTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Add"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Add_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Constant(1),
                new Float64Add(), new End());

            Assert.Equal(1, exports.Test(0));
            Assert.Equal(6, exports.Test(5));
        }
    }
}
