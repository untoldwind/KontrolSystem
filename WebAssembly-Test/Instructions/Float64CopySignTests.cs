using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64CopySign"/> instruction.
    /// </summary>
    public class Float64CopySignTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64CopySign"/> instruction.
        /// </summary>
        [Fact]
        public void Float64CopySign_Compiled() {
            var exports = CompilerTestBase2<double>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Float64CopySign(), new End());

            Assert.Equal(1, exports.Test(1, +2));
            Assert.Equal(-1, exports.Test(1, -2));
            Assert.Equal(-double.PositiveInfinity, exports.Test(double.PositiveInfinity, -2));
            Assert.Equal(-double.NaN, exports.Test(double.NaN, -2));
        }
    }
}
