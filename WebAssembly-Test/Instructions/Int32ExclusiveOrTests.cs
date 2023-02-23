using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32ExclusiveOr"/> instruction.
    /// </summary>
    public class Int32ExclusiveOrTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32ExclusiveOr"/> instruction.
        /// </summary>
        [Fact]
        public void Int32ExclusiveOr_Compiled() {
            const int or = 0xF;

            var exports = CompilerTestBase<int>.CreateInstance(new LocalGet(0), new Int32Constant(or),
                new Int32ExclusiveOr(), new End());

            foreach (var value in new[] { 0x00, 0x0F, 0xF0, 0xFF, }) Assert.Equal(value ^ or, exports.Test(value));
        }
    }
}
