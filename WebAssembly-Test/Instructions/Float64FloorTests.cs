using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Floor"/> instruction.
    /// </summary>
    public class Float64FloorTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Floor"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Floor_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Floor(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Floor(value), exports.Test(value));
        }
    }
}
