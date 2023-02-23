using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Extend8Signed"/> instruction.
    /// </summary>
    public class Int64Extend8SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Extend8Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Extend8Signed_Compiled() {
            var exports = ConversionTestBase<long, long>.CreateInstance(
                new LocalGet(0), new Int64Extend8Signed(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/7526564b56c30250b66504fe795e9c1e88a938af/test/core/i64.wast
            Assert.Equal(0, exports.Test(0));
            Assert.Equal(127, exports.Test(0x7f));
            Assert.Equal(-128, exports.Test(0x80));
            Assert.Equal(-1, exports.Test(0xff));
            Assert.Equal(0, exports.Test(0x0123456789abcd00));
            Assert.Equal(-0x80, exports.Test(unchecked((long)0xfedcba9876543280)));
            Assert.Equal(-1, exports.Test(-1));
        }
    }
}
