﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64RotateLeft"/> instruction.
    /// </summary>
    public class Int64RotateLeftTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64RotateLeft"/> instruction.
        /// </summary>
        [Fact]
        public void Int64RotateLeft_Compiled() {
            var exports = CompilerTestBase2<long>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int64RotateLeft(), new End());

            //Test cases from https://github.com/WebAssembly/spec/blob/f1b89dfaf379060c7e35eb90b7daeb14d4ade3f7/test/core/i64.wast
            Assert.Equal(2, exports.Test(1, 1));
            Assert.Equal(1, exports.Test(1, 0));
            Assert.Equal(-1, exports.Test(-1, 1));
            Assert.Equal(1, exports.Test(1, 64));
            Assert.Equal(0x579b30ec048d159d, exports.Test(unchecked((long)0xabcd987602468ace), 1));
            Assert.Equal(unchecked((long)0xe000000dc000000f), exports.Test(unchecked((long)0xfe000000dc000000), 4));
            Assert.Equal(0x013579a2469deacf, exports.Test(unchecked((long)0xabcd1234ef567809), 53));
            Assert.Equal(0x55e891a77ab3c04e, exports.Test(unchecked((long)0xabd1234ef567809c), 63));
            Assert.Equal(0x013579a2469deacf, exports.Test(unchecked((long)0xabcd1234ef567809), 0xf5));
            Assert.Equal(unchecked((long)0xcf013579ae529dea),
                exports.Test(unchecked((long)0xabcd7294ef567809), unchecked((long)0xffffffffffffffed)));
            Assert.Equal(0x55e891a77ab3c04e,
                exports.Test(unchecked((long)0xabd1234ef567809c), unchecked((long)0x800000000000003f)));
            Assert.Equal(unchecked((long)0x8000000000000000), exports.Test(1, 63));
            Assert.Equal(1, exports.Test(unchecked((long)0x8000000000000000), 1));
        }
    }
}