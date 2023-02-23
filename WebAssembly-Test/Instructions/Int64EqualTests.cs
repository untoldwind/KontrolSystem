﻿using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Equal"/> instruction.
    /// </summary>
    public class Int64EqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Equal"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Equal_Compiled() {
            var exports = ComparisonTestBase<long>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int64Equal(), new End());

            var values = new[] {
                -1, 0, 1, 0x00, 0x0F, 0xF0, 0xFF, byte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue,
                int.MinValue, int.MaxValue, uint.MaxValue, long.MinValue, long.MaxValue,
            };

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand == value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value == comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
