using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {
    public class ElseTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Else"/> instruction.
        /// </summary>
        [Fact]
        public void Else_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0),
                new If(),
                new Int32Constant(3),
                new Return(),
                new Else(),
                new Int32Constant(2),
                new Return(),
                new End(),
                new Int32Constant(1),
                new End());

            Assert.Equal(2, exports.Test(0));
            Assert.Equal(3, exports.Test(1));
        }


        /// <summary>
        /// Tests compilation and execution of the <see cref="Else"/> instruction.
        /// </summary>
        [Fact]
        public void Else_Compiled_CarriedValue() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new LocalGet(0),
                new If(BlockType.Int32),
                new Int32Constant(1),
                new Else(),
                new Int32Constant(2),
                new End(),
                new End());

            Assert.Equal(2, exports.Test(0));
            Assert.Equal(1, exports.Test(1));
            Assert.Equal(1, exports.Test(2));
        }
    }
}
