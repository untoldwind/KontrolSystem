using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Extend16Signed"/> instruction.
    /// </summary>
    public class Int64Extend16SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Extend16Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Extend16Signed_Compiled() {
            var exports = ConversionTestBase<long, long>.CreateInstance(
                new LocalGet(0), new Int64Extend16Signed(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/i64.wast
            Assert.Equal(0, exports.Test(0));
            Assert.Equal(32767, exports.Test(0x7fff));
            Assert.Equal(-32768, exports.Test(0x8000));
            Assert.Equal(-1, exports.Test(0xffff));
            Assert.Equal(0, exports.Test(0x0123456789abc0000));
            Assert.Equal(-0x8000, exports.Test(unchecked((long)0xfedcba9876548000)));
            Assert.Equal(-1, exports.Test(-1));
        }
    }
}
