using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Return"/> instruction.
    /// </summary>
    public class ReturnTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction.
        /// </summary>
        [Fact]
        public void Return_Compiled() {
            AssemblyBuilder.CreateInstance<CompilerTestBaseVoid0>("Test", null, new Return(), new End()).Test();
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction with a value.
        /// </summary>
        [Fact]
        public void Return_Compiled_WithValue() {
            Assert.Equal<int>(4,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32, new Int32Constant(4),
                    new Return(), new End()).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction where two values are returned when one is expected.
        /// </summary>
        [Fact]
        public void Return_Compiled_IncorrectStack_Expect1Actual2() {
            Assert.Equal<int>(2,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32, new Int32Constant(1),
                    new Int32Constant(2), new Return(), new End()).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction where three values are returned when one is expected.
        /// </summary>
        [Fact]
        public void Return_Compiled_IncorrectStack_Expect1Actual3() {
            Assert.Equal<int>(3,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32, new Int32Constant(1),
                    new Int32Constant(2), new Int32Constant(3), new Return(), new End()).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction where one value are returned when none are expected.
        /// </summary>
        [Fact]
        public void Return_Compiled_IncorrectStack_Expect0Actual1() {
            AssemblyBuilder.CreateInstance<CompilerTestBaseVoid0>("Test", null, new Int32Constant(1), new Return(), new End()).Test();
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Return"/> instruction where two values are returned when none are expected.
        /// </summary>
        [Fact]
        public void Return_Compiled_IncorrectStack_Expect0Actual2() {
            AssemblyBuilder.CreateInstance<CompilerTestBaseVoid0>("Test", null, new Int32Constant(1), new Int32Constant(2),
                new Return(), new End()).Test();
        }
    }
}
