using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32CountLeadingZeroes"/> instruction.
    /// </summary>
    public class Int32CountLeadingZeroesTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32CountLeadingZeroes"/> instruction.
        /// </summary>
        [Fact]
        public void Int32CountLeadingZeroes_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0), new Int32CountLeadingZeroes(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/f1b89dfaf379060c7e35eb90b7daeb14d4ade3f7/test/core/i32.wast
            Assert.Equal(0, exports.Test(unchecked((int)0xffffffff)));
            Assert.Equal(32, exports.Test(0));
            Assert.Equal(16, exports.Test(0x00008000));
            Assert.Equal(24, exports.Test(0xff));
            Assert.Equal(0, exports.Test(unchecked((int)0x80000000)));
            Assert.Equal(31, exports.Test(1));
            Assert.Equal(30, exports.Test(2));
            Assert.Equal(1, exports.Test(0x7fffffff));
        }
    }
}
