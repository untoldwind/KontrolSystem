using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Ceiling"/> instruction.
    /// </summary>
    public class Float64CeilingTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Ceiling"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Ceiling_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Ceiling(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Ceiling(value), exports.Test(value));
        }
    }
}
