using Xunit;
using System;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Constant"/> instruction.
    /// </summary>
    public class Float32ConstantTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Constant"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Constant_Compiled() {
            foreach (var sample in new float[] {
                         -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, //Dedicated .NET Opcodes
                         byte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, int.MinValue, int.MaxValue,
                         uint.MaxValue, long.MinValue, long.MaxValue, (float)Math.PI, -(float)Math.PI,
                     }) {
                Assert.Equal<float>(sample,
                    AssemblyBuilder.CreateInstance<CompilerTestBase0<float>>("Test", WebAssemblyValueType.Float32,
                        new Float32Constant(sample), new End()).Test());
            }
        }
    }
}
