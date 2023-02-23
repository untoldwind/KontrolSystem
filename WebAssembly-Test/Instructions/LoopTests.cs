using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Loop"/> instruction.
    /// </summary>
    public class LoopTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Loop"/> instruction.
        /// </summary>
        [Fact]
        public void Loop_Compiled() {
            var exports = CompilerTestBase2<int>.CreateInstance(new Block(BlockType.Empty), new Loop(BlockType.Empty),
                new LocalGet(0), new Int32Constant(1), new Int32Add(), new LocalSet(0), new LocalGet(1),
                new Int32Constant(1), new Int32Add(), new LocalSet(1), new LocalGet(1), new If(BlockType.Empty),
                new Branch(2), new Else(), new Branch(1), new End(), //if

                new End(), //loop
                new End(), //block
                new LocalGet(0), new End());

            Assert.Equal(11, exports.Test(10, -2));
            Assert.Equal(12, exports.Test(10, -1));
            Assert.Equal(11, exports.Test(10, 0));
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Loop"/> instruction that yields a value.
        /// </summary>
        [Fact]
        public void Branch_LoopValue() {
            var exports = AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                new Loop(BlockType.Int32), new Int32Constant(7), new End(), new End());

            Assert.Equal<int>(7, exports.Test());
        }
    }
}
