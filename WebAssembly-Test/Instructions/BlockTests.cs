using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {
    /// <summary>
    /// Tests the <see cref="Block"/> instruction.
    /// </summary>
    public class BlockTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Block"/> instruction.
        /// </summary>
        [Fact]
        public void Block_Compiled() {
            var exports = AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test",
                WebAssemblyValueType.Int32,
                new Block(BlockType.Empty),
                new Block(BlockType.Empty),
                new End(),
                new Block(BlockType.Empty),
                new Block(BlockType.Empty),
                new End(),
                new End(),
                new End(),
                new Int32Constant(6),
                new End());

            Assert.Equal(6, exports.Test());
        }

        /// <summary>
        /// Tests compilation and execution of a <see cref="Block"/> instruction that returns a value.
        /// </summary>
        [Fact]
        public void Block_Returns() {
            var exports = AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test",
                WebAssemblyValueType.Int32,
                new Block(BlockType.Int32),
                new Int32Constant(5),
                new End(),
                new End());

            Assert.Equal(5, exports.Test());
        }

        /// <summary>
        /// Tests that the <see cref="BlockTypeInstruction.ToString"/> overload on <see cref="Block"/> provides the correct WAT formatted result.
        /// </summary>
        [Fact]
        public void Block_ToStringAccuracy() {
            Assert.Equal("block", new Block().ToString());
            Assert.Equal("block", new Block(BlockType.Empty).ToString());
            Assert.Equal("block i32", new Block(BlockType.Int32).ToString());
            Assert.Equal("block i64", new Block(BlockType.Int64).ToString());
            Assert.Equal("block f32", new Block(BlockType.Float32).ToString());
            Assert.Equal("block f64", new Block(BlockType.Float64).ToString());
        }
    }

}
