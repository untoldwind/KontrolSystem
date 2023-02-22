using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {
    /// <summary>
    /// Tests the <see cref="BranchIf"/> instruction.
    /// </summary>
    public class BranchIfTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="BranchIf"/> instruction.
        /// </summary>
        [Fact]
        public void BranchIf_Compiled() {
            var exports = CompilerTestBase<int>.CreateInstance(
                new Block(BlockType.Empty),
                new LocalGet(0),
                new BranchIf(0),
                new Int32Constant(2),
                new Return(),
                new End(),
                new Int32Constant(1),
                new End());

            Assert.Equal(2, exports.Test(0));
            Assert.Equal(1, exports.Test(1));
        }

        /// <summary>
        /// Tests compilation of the <see cref="BranchIf"/> and <see cref="Loop"/> instructions that yields a value with no way for it to end.
        /// </summary>
        [Fact]
        public void BranchIf_LoopInfiniteWithValue() {
            _ = AssemblyBuilder.CreateInstance<object>("Test",
                WebAssemblyValueType.Int32,
                new Loop(BlockType.Int32),
                new Int32Constant(3),
                new Int32Constant(1),
                new BranchIf(),
                new End(),
                new End());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="BranchIf"/> and <see cref="Loop"/> instructions that yields a value.
        /// </summary>
        [Fact]
        public void BranchIf_LoopBreakWithValue() {
            var exports = AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test",
                WebAssemblyValueType.Int32,
                new Loop(BlockType.Int32),
                new Int32Constant(3),
                new Int32Constant(0),
                new BranchIf(),
                new End(),
                new End());

            Assert.Equal(3, exports.Test());
        }
    }

}
