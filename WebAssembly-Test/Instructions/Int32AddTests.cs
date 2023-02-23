using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32Add"/> instruction.
    /// </summary>
    public class Int32AddTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Add"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Add_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0), new Int32Constant(1), new Int32Add(), new End());

            Assert.Equal(1, exports.Test(0));
            Assert.Equal(6, exports.Test(5));
        }
    }
}
