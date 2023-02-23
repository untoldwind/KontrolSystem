using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="LocalTee"/> instruction.
    /// </summary>
    public class LocalTeeTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="LocalTee"/> instruction.
        /// </summary>
        [Fact]
        public void TeeLocal_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new LocalTee(0), new End());

            Assert.Equal(3, exports.Test(3));
            Assert.Equal(-1, exports.Test(-1));
        }
    }
}
