using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64SquareRoot"/> instruction.
    /// </summary>
    public class Float64SquareRootTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64SquareRoot"/> instruction.
        /// </summary>
        [Fact]
        public void Float64SquareRoot_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64SquareRoot(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Sqrt(value), exports.Test(value));
        }
    }
}
