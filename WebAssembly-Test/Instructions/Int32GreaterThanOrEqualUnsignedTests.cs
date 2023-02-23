using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32GreaterThanOrEqualUnsigned"/> instruction.
    /// </summary>
    public class Int32GreaterThanOrEqualUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32GreaterThanOrEqualUnsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32GreaterThanOrEqualUnsigned_Compiled() {
            var exports = ComparisonTestBase<int>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Int32GreaterThanOrEqualUnsigned(), new End());

            var values = new uint[] {
                0, 1, 0x00, 0x0F, 0xF0, 0xFF, byte.MaxValue, ushort.MaxValue, int.MaxValue, uint.MaxValue,
            };

            foreach (var comparand in values) {
                foreach (var value in values)
                    Assert.Equal(comparand >= value, exports.Test((int)comparand, (int)value) != 0);

                foreach (var value in values)
                    Assert.Equal(value >= comparand, exports.Test((int)value, (int)comparand) != 0);
            }
        }
    }
}
