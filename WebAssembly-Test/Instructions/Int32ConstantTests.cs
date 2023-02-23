using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32Constant"/> instruction.
    /// </summary>
    public class Int32ConstantTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Constant"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Constant_Compiled() {
            foreach (var sample in new[] {
                         -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, //Dedicated .NET Opcodes
                         byte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, int.MinValue, int.MaxValue,
                     }) {
                Assert.Equal<int>(sample,
                    AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                        new Int32Constant(sample), new End()).Test());
            }
        }
    }
}
