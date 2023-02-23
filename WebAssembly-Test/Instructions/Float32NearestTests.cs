using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Nearest"/> instruction.
    /// </summary>
    public class Float32NearestTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Nearest"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Nearest_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(new LocalGet(0), new Float32Nearest(), new End());

            foreach (var value in Samples.Single)
                Assert.Equal((float)Math.Round(value, MidpointRounding.ToEven), exports.Test(value));
        }
    }
}
