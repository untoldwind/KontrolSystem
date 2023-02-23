using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64CountOneBits"/> instruction.
    /// </summary>
    public class Int64CountOneBitsTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64CountOneBits"/> instruction.
        /// </summary>
        [Fact]
        public void Int64CountOneBits_Compiled() {
            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64CountOneBits(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/f1b89dfaf379060c7e35eb90b7daeb14d4ade3f7/test/core/i64.wast
            Assert.Equal(64, exports.Test(-1));
            Assert.Equal(0, exports.Test(0));
            Assert.Equal(1, exports.Test(0x00008000));
            Assert.Equal(4, exports.Test(unchecked((long)0x8000800080008000)));
            Assert.Equal(63, exports.Test(0x7fffffffffffffff));
            Assert.Equal(32, exports.Test(unchecked((long)0xAAAAAAAA55555555)));
            Assert.Equal(32, exports.Test(unchecked((long)0x99999999AAAAAAAA)));
            Assert.Equal(48, exports.Test(unchecked((long)0xDEADBEEFDEADBEEF)));
        }
    }
}
