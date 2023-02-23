using Xunit;
using System;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Constant"/> instruction.
    /// </summary>
    public class Float64ConstantTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Constant"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Constant_Compiled() {
            foreach (var sample in new double[] {
                         -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, //Dedicated .NET Opcodes
                         byte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, int.MinValue, int.MaxValue,
                         uint.MaxValue, long.MinValue, long.MaxValue, Math.PI, -Math.PI,
                     }) {
                Assert.Equal<double>(sample,
                    AssemblyBuilder.CreateInstance<CompilerTestBase0<double>>("Test", WebAssemblyValueType.Float64,
                        new Float64Constant(sample), new End()).Test());
            }
        }
    }
}
