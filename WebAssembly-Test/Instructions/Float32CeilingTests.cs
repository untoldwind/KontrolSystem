using System;
using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Ceiling"/> instruction.
    /// </summary>
    public class Float32CeilingTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Ceiling"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Ceiling_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Ceiling(), new End());

            foreach (var value in Samples.Single) Assert.Equal((float)Math.Ceiling(value), exports.Test(value));
        }
    }
}
