﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32RotateLeft"/> instruction.
    /// </summary>
    public class Int32RotateLeftTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32RotateLeft"/> instruction.
        /// </summary>
        [Fact]
        public void Int32RotateLeft_Compiled() {
            var exports = CompilerTestBase2<int>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int32RotateLeft(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/f1b89dfaf379060c7e35eb90b7daeb14d4ade3f7/test/core/i32.wast
            Assert.Equal(2, exports.Test(1, 1));
            Assert.Equal(1, exports.Test(1, 0));
            Assert.Equal(-1, exports.Test(-1, 1));
            Assert.Equal(1, exports.Test(1, 32));
            Assert.Equal(0x579b30ed, exports.Test(unchecked((int)0xabcd9876), 1));
            Assert.Equal(unchecked((int)0xe00dc00f), exports.Test(unchecked((int)0xfe00dc00), 4));
            Assert.Equal(0x183a5c76, exports.Test(unchecked((int)0xb0c1d2e3), 5));
            Assert.Equal(0x00100000, exports.Test(0x00008000, 37));
            Assert.Equal(0x183a5c76, exports.Test(unchecked((int)0xb0c1d2e3), 0xff05));
            Assert.Equal(0x579beed3, exports.Test(0x769abcdf, unchecked((int)0xffffffed)));
            Assert.Equal(0x579beed3, exports.Test(0x769abcdf, unchecked((int)0x8000000d)));
            Assert.Equal(unchecked((int)0x80000000), exports.Test(1, 31));
            Assert.Equal(1, exports.Test(unchecked((int)0x80000000), 1));
        }
    }
}
