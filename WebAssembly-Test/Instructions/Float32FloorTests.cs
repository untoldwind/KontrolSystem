using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Floor"/> instruction.
    /// </summary>
    public class Float32FloorTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Floor"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Floor_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Floor(), new End());

            foreach (var value in Samples.Single) Assert.Equal((float)Math.Floor(value), exports.Test(value));
        }
    }
}
