using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Or"/> instruction.
    /// </summary>
    public class Int64OrTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Or"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Or_Compiled() {
            const int comparand = 0xF;

            var exports = CompilerTestBase<long>.CreateInstance(new LocalGet(0), new Int64Constant(comparand),
                new Int64Or(), new End());

            foreach (var value in new long[] { 0x00, 0x0F, 0xF0, 0xFF, })
                Assert.Equal(value | comparand, exports.Test(value));
        }
    }
}
