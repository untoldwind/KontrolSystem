using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Truncate"/> instruction.
    /// </summary>
    public class Float32TruncateTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Truncate"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Truncate_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Truncate(), new End());

            foreach (var value in Samples.Single) Assert.Equal((float)Math.Truncate(value), exports.Test(value));
        }
    }
}
