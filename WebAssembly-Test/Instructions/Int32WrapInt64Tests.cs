using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32WrapInt64"/> instruction.
    /// </summary>
    public class Int32WrapInt64Tests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32WrapInt64"/> instruction.
        /// </summary>
        [Fact]
        public void Int32WrapInt64_Compiled() {
            var exports = ConversionTestBase<long, int>.CreateInstance(
                new LocalGet(0), new Int32WrapInt64(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/c4774b47d326e4114f96232f1389a555639d7348/test/core/conversions.wast
            Assert.Equal(-1, exports.Test(-1));
            Assert.Equal(-100000, exports.Test(-100000));
            Assert.Equal(unchecked((int)0x80000000), exports.Test(0x80000000));
            Assert.Equal(0x7fffffff, exports.Test(unchecked((long)0xffffffff7fffffff)));
            Assert.Equal(0x00000000, exports.Test(unchecked((long)0xffffffff00000000)));
            Assert.Equal(unchecked((int)0xffffffff), exports.Test(unchecked((long)0xfffffffeffffffff)));
            Assert.Equal(0x00000001, exports.Test(unchecked((long)0xffffffff00000001)));
            Assert.Equal(0, exports.Test(0));
            Assert.Equal(unchecked((int)0x9ABCDEF0), exports.Test(0x123456789ABCDEF0));
            Assert.Equal(unchecked((int)0xffffffff), exports.Test(0x00000000ffffffff));
            Assert.Equal(unchecked((int)0x0000000100000000), exports.Test(0x00000000));
            Assert.Equal(unchecked((int)0x0000000100000001), exports.Test(0x00000001));
        }
    }
}
