using Xunit;
using System;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Equal"/> instruction.
    /// </summary>
    public class Float32EqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Equal"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Equal_Compiled() {
            var exports = ComparisonTestBase<float>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Float32Equal(), new End());

            var values = new[] {
                0.0f, 1.0f, -1.0f, -(float)Math.PI, (float)Math.PI, (float)double.NaN, (float)double.NegativeInfinity,
                (float)double.PositiveInfinity, (float)double.Epsilon, -(float)double.Epsilon,
            };

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand == value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value == comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
