using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="If"/> instruction.
    /// </summary>
    public class IfTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="If"/> instruction.
        /// </summary>
        [Fact]
        public void If_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new If(), new Int32Constant(3),
                new Return(), new End(), new Int32Constant(2), new End());

            Assert.Equal(2, exports.Test(0));
            Assert.Equal(3, exports.Test(1));
        }
    }
}
