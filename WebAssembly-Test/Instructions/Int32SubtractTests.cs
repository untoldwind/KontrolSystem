using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32Subtract"/> instruction.
    /// </summary>
    public class Int32SubtractTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Subtract"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Subtract_Compiled() {
            const int comparand = 0x8;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(comparand),
                new Int32Subtract(), new End());

            foreach (var value in new[] { 0x00, 0x0F, 0xF0, 0xFF, })
                Assert.Equal(value - comparand, exports.Test(value));
        }
    }
}
