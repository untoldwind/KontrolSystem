using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32NotEqual"/> instruction.
    /// </summary>
    public class Int32NotEqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32NotEqual"/> instruction.
        /// </summary>
        [Fact]
        public void Int32NotEqual_Compiled() {
            var exports = ComparisonTestBase<int>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int32NotEqual(), new End());

            var values = Samples.Int32;

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand != value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value != comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
