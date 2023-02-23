using System;
using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Truncate"/> instruction.
    /// </summary>
    public class Float64TruncateTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Truncate"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Truncate_Compiled() {
            var exports = CompilerTestBase<double>.CreateInstance(new LocalGet(0), new Float64Truncate(), new End());

            foreach (var value in new[] { 1f, -1f, -Math.PI, Math.PI })
                Assert.Equal(Math.Truncate(value), exports.Test(value));
        }
    }
}
