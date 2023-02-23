using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Store"/> instruction.
    /// </summary>
    public class Float32StoreTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Store"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Store_Compiled_Offset0() {
            var compiled = MemoryWriteTestBase<float>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Float32Store(), new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, (float)Math.PI);
                Assert.Equal(1078530011, Marshal.ReadInt32(memory.Start));
                Assert.Equal(4213007, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(16457, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(64, Marshal.ReadInt32(memory.Start, 3));

                exports.Test((int)Memory.PageSize - 4, 1);

                Assert.Equal(1065353216, Marshal.ReadInt32(memory.Start, (int)Memory.PageSize - 4));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3, 0));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2, 0));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(4u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Store"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Store_Compiled_Offset1() {
            var compiled = MemoryWriteTestBase<float>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Float32Store() { Offset = 1 }, new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, (float)Math.PI);
                Assert.Equal(1225775872, Marshal.ReadInt32(memory.Start));
                Assert.Equal(1078530011, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(4213007, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(16457, Marshal.ReadInt32(memory.Start, 3));
                Assert.Equal(64, Marshal.ReadInt32(memory.Start, 4));

                exports.Test((int)Memory.PageSize - 4 - 1, 1);

                Assert.Equal(1065353216, Marshal.ReadInt32(memory.Start, (int)Memory.PageSize - 4));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 4, 0));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3, 0));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(4u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }
    }
}
