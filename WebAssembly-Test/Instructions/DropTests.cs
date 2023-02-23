using Xunit;
using WebAssembly.Instructions;
using WebAssembly.Runtime;

namespace WebAssembly.Test.Instructions {
    public class DropTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Drop"/> instruction.
        /// </summary>
        [Fact]
        public void Drop_Compiled() {
            Assert.Equal<int>(1, AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32, new Int32Constant(1), new Int32Constant(2), new Drop(), new End()).Test());

            var stackTooSmall = Assert.Throws<StackTooSmallException>(() => AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", null, new Drop(), new End()).Test());
            Assert.Equal(OpCode.Drop, stackTooSmall.OpCode);
            Assert.Equal(1, stackTooSmall.Minimum);
            Assert.Equal(0, stackTooSmall.Actual);
        }
    }

}
