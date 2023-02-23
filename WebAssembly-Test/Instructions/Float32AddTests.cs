using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {
    public class Float32AddTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Add"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Add_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(
                new LocalGet(0),
                new Float32Constant(1),
                new Float32Add(),
                new End());

            Assert.Equal(1, exports.Test(0));
            Assert.Equal(6, exports.Test(5));
        }
    }

}
