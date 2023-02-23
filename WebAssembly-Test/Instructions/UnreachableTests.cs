using Xunit;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Unreachable"/> instruction.
    /// </summary>
    public class UnreachableTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Unreachable"/> instruction.
        /// </summary>
        [Fact]
        public void Unreachable_Compiled() {
            Assert.Throws<UnreachableException>(() => {
                AssemblyBuilder.CreateInstance<CompilerTestBaseVoid0>("Test", null, new Unreachable(), new End()).Test();
            });
        }
    }
}
