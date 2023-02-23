using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64ExtendInt32Signed"/> instruction.
    /// </summary>
    public class Int64ExtendInt32SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64ExtendInt32Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64ExtendSignedInt32_Compiled() {
            var exports = ConversionTestBase<int, long>.CreateInstance(
                new LocalGet(0), new Int64ExtendInt32Signed(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/c4774b47d326e4114f96232f1389a555639d7348/test/core/conversions.wast
            Assert.Equal(0, exports.Test(0));
            Assert.Equal(10000, exports.Test(10000));
            Assert.Equal(-10000, exports.Test(-10000));
            Assert.Equal(-1, exports.Test(-1));
            Assert.Equal(0x000000007fffffff, exports.Test(0x7fffffff));
            Assert.Equal(unchecked((long)0xffffffff80000000), exports.Test(unchecked((int)0x80000000)));
        }
    }
}
