using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Absolute"/> instruction.
    /// </summary>
    public class Float64AbsoluteTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Absolute"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Absolute_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Absolute(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Abs(value), exports.Test(value));
        }
    }
}
