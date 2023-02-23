using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Constant"/> instruction.
    /// </summary>
    public class Int64ConstantTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Constant"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Constant_Compiled() {
            foreach (var sample in new[] {
                         -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, //Dedicated .NET Opcodes
                         byte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, int.MinValue, int.MaxValue,
                         uint.MaxValue, long.MinValue, long.MaxValue,
                     }) {
                Assert.Equal<long>(sample,
                    AssemblyBuilder.CreateInstance<CompilerTestBase0<long>>("Test", WebAssemblyValueType.Int64,
                        new Int64Constant(sample), new End()).Test());
            }
        }
    }
}
