using Xunit;
using System;
using WebAssembly.Runtime;

namespace WebAssembly.Test.Runtime {
    /// <summary>
    /// Tests the <see cref="UnmanagedMemory"/> class.
    /// </summary>
    public class UnmanagedMemoryTests {
        /// <summary>
        /// Tests that memory can be grown from a starting point of zero.
        /// </summary>
        [Fact]
        public void UnmanagedMemory_GrowFromZero() {
            Assert.Equal(0u, new UnmanagedMemory(0, 1).Grow(1));
        }

        /// <summary>
        /// Tests that a disposed instance can't be revived by the Grow function.
        /// </summary>
        [Fact]
        public void UnamangedMemory_BlockGrowWhenDisposed() {
            var memory = new UnmanagedMemory(0, 1);
            memory.Dispose();
            Assert.Throws<ObjectDisposedException>(() => memory.Grow(1));
        }
    }

}
