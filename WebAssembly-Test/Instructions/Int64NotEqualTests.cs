using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64NotEqual"/> instruction.
    /// </summary>
    public class Int64NotEqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64NotEqual"/> instruction.
        /// </summary>
        [Fact]
        public void Int64NotEqual_Compiled() {
            var exports = ComparisonTestBase<long>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int64NotEqual(), new End());

            var values = Samples.Int64;

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand != value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value != comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
