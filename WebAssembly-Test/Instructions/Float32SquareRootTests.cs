using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32SquareRoot"/> instruction.
    /// </summary>
    public class Float32SquareRootTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32SquareRoot"/> instruction.
        /// </summary>
        [Fact]
        public void Float32SquareRoot_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32SquareRoot(), new End());

            foreach (var value in Samples.Single) Assert.Equal((float)Math.Sqrt(value), exports.Test(value));
        }
    }
}
