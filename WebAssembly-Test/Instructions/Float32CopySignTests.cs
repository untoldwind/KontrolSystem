using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32CopySign"/> instruction.
    /// </summary>
    public class Float32CopySignTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32CopySign"/> instruction.
        /// </summary>
        [Fact]
        public void Float32CopySign_Compiled() {
            var exports = CompilerTestBase2<float>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Float32CopySign(), new End());

            Assert.Equal(1, exports.Test(1, +2));
            Assert.Equal(-1, exports.Test(1, -2));
            Assert.Equal(-float.PositiveInfinity, exports.Test(float.PositiveInfinity, -2));
            Assert.Equal(-float.NaN, exports.Test(float.NaN, -2));
        }
    }
}
