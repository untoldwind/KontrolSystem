using System;
using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Nearest"/> instruction.
    /// </summary>
    public class Float64NearestTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Nearest"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Nearest_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Nearest(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Round(value, MidpointRounding.ToEven), exports.Test(value));
        }
    }
}
