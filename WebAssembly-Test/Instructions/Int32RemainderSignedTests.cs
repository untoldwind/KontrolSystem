using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32RemainderSigned"/> instruction.
    /// </summary>
    public class Int32RemainderSignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32RemainderSigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32RemainderSigned_Compiled() {
            const int divisor = 0xF;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(divisor),
                new Int32RemainderSigned(), new End());

            foreach (var value in new[] { 0x00, 0x0F, 0xF0, 0xFF, }) Assert.Equal(value % divisor, exports.Test(value));
        }
    }
}
