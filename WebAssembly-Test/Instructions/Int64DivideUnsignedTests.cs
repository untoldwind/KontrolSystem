using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64DivideUnsigned"/> instruction.
    /// </summary>
    public class Int64DivideUnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Add"/> instruction.
        /// </summary>
        [Fact]
        public void Int64DivideUnsigned_Compiled() {
            const uint divisor = 2;

            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64Constant(divisor),
                new Int64DivideUnsigned(), new End());

            foreach (var value in new ulong[] { 0, 1, 2, 3, 4, 5, })
                Assert.Equal(value / divisor, (ulong)exports.Test((long)value));
        }
    }
}
