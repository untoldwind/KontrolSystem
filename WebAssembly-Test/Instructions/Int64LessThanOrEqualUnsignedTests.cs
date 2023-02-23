using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64LessThanOrEqualUnsigned"/> instruction.
    /// </summary>
    public class Int64LessThanOrEqualUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64LessThanOrEqualUnsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64LessThanOrEqualUnsigned_Compiled() {
            var exports = ComparisonTestBase<long>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Int64LessThanOrEqualUnsigned(), new End());

            var values = new ulong[] {
                0, 1, 0x00, 0x0F, 0xF0, 0xFF, byte.MaxValue, ushort.MaxValue, int.MaxValue, uint.MaxValue,
                long.MaxValue, ulong.MaxValue,
            };

            foreach (var comparand in values) {
                foreach (var value in values)
                    Assert.Equal(comparand <= value, exports.Test((long)comparand, (long)value) != 0);

                foreach (var value in values)
                    Assert.Equal(value <= comparand, exports.Test((long)value, (long)comparand) != 0);
            }
        }
    }
}
