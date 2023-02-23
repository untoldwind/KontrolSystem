using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64RemainderUnsigned"/> instruction.
    /// </summary>
    public class Int64RemainderUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64RemainderUnsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64RemainderUnsigned_Compiled() {
            const uint divisor = 0xF;

            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64Constant(divisor),
                new Int64RemainderUnsigned(), new End());

            foreach (var value in new ulong[] { 0x00, 0x0F, 0xF0, 0xFF, })
                Assert.Equal(value % divisor, (ulong)exports.Test((long)value));
        }
    }
}
