﻿using Xunit;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32NotEqual"/> instruction.
    /// </summary>
    public class Float32NotEqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32NotEqual"/> instruction.
        /// </summary>
        [Fact]
        public void Float32NotEqual_Compiled() {
            var exports = ComparisonTestBase<float>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Float32NotEqual(), new End());

            var values = Samples.Single;

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand != value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value != comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
