using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32CountTrailingZeroes"/> instruction.
    /// </summary>
    public class Int32CountTrailingZeroesTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32CountTrailingZeroes"/> instruction.
        /// </summary>
        [Fact]
        public void Int32CountTrailingZeroes_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0), new Int32CountTrailingZeroes(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/f1b89dfaf379060c7e35eb90b7daeb14d4ade3f7/test/core/i32.wast
            Assert.Equal(0, exports.Test(-1));
            Assert.Equal(32, exports.Test(0));
            Assert.Equal(15, exports.Test(0x00008000));
            Assert.Equal(16, exports.Test(0x00010000));
            Assert.Equal(31, exports.Test(unchecked((int)0x80000000)));
            Assert.Equal(0, exports.Test(0x7fffffff));
        }
    }
}
