using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32And"/> instruction.
    /// </summary>
    public class Int32AndTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32And"/> instruction.
        /// </summary>
        [Fact]
        public void Int32And_Compiled() {
            const int and = 0xF;

            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0), new Int32Constant(and), new Int32And(), new End());

            foreach (var value in new[] { 0x00, 0x0F, 0xF0, 0xFF, }) Assert.Equal(value & and, exports.Test(value));
        }
    }
}
