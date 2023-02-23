using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64EqualZero"/> instruction.
    /// </summary>
    public class Int64EqualZeroTests {
        /// <summary>
        /// A simple test class.
        /// </summary>
        public abstract class TestClass {
            /// <summary>
            /// A simple test method.
            /// </summary>
            public abstract int Test(long value);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64EqualZero"/> instruction.
        /// </summary>
        [Fact]
        public void Int64EqualZero_Compiled() {
            var exports = AssemblyBuilder.CreateInstance<TestClass>("Test", WebAssemblyValueType.Int32,
                new[] { WebAssemblyValueType.Int64, }, new LocalGet(0), new Int64EqualZero(), new End());

            foreach (var value in Samples.Int64) Assert.Equal(value == 0, exports.Test(value) != 0);
        }
    }
}
