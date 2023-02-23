using Xunit;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="NoOperation"/> instruction.
    /// </summary>
    public class NoOperationTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="NoOperation"/> instruction.
        /// </summary>
        [Fact]
        public void NoOperation_Compiled() {
            AssemblyBuilder.CreateInstance<CompilerTestBaseVoid0>("Test", null, new NoOperation(), new End()).Test();
        }
    }
}
