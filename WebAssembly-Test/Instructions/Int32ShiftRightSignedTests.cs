using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32ShiftRightSigned"/> instruction.
    /// </summary>
    public class Int32ShiftRightSignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32ShiftRightSigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32ShiftRightSigned_Compiled() {
            const int amount = 0xF;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(amount),
                new Int32ShiftRightSigned(), new End());

            foreach (var value in new[] { 0x00, 0x0F, 0xF0, 0xFF, }) Assert.Equal(value >> amount, exports.Test(value));
        }
    }
}
