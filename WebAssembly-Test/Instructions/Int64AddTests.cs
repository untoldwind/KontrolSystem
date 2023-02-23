using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Add"/> instruction.
    /// </summary>
    public class Int64AddTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Add"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Add_Compiled() {
            var exports = CompilerTestBase<long>.CreateInstance(
                new LocalGet(0), new Int64Constant(1), new Int64Add(), new End());

            Assert.Equal(1, exports.Test(0));
            Assert.Equal(6, exports.Test(5));
        }
    }
}
