using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Subtract"/> instruction.
    /// </summary>
    public class Float64SubtractTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Subtract"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Subtract_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Constant(1),
                new Float64Subtract(), new End());

            Assert.Equal(-1, exports.Test(0));
            Assert.Equal(4, exports.Test(5));
        }
    }
}
