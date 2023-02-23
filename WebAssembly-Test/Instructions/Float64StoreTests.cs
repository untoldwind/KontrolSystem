using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;
using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Store"/> instruction.
    /// </summary>
    public class Float64StoreTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Store"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Store_Compiled_Offset0() {
            var compiled = MemoryWriteTestBase<double>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Float64Store(), new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, Math.PI);
                Assert.Equal(1413754136, Marshal.ReadInt32(memory.Start));
                Assert.Equal(-78363603, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(570119236, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(153221972, Marshal.ReadInt32(memory.Start, 3));

                exports.Test((int)Memory.PageSize - 8, 1);

                Assert.Equal(4607182418800017408, Marshal.ReadInt64(memory.Start, (int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 7, 0));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 6, 0));
                Assert.Equal(Memory.PageSize - 6, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 5, 0));
                Assert.Equal(Memory.PageSize - 5, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 4, 0));
                Assert.Equal(Memory.PageSize - 4, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3, 0));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2, 0));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Store"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Store_Compiled_Offset1() {
            var compiled = MemoryWriteTestBase<double>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Float64Store() { Offset = 1 }, new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, Math.PI);
                Assert.Equal(1143805952, Marshal.ReadInt32(memory.Start));
                Assert.Equal(1413754136, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(-78363603, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(570119236, Marshal.ReadInt32(memory.Start, 3));
                Assert.Equal(153221972, Marshal.ReadInt32(memory.Start, 4));

                exports.Test((int)Memory.PageSize - 8 - 1, 1);

                Assert.Equal(4607182418800017408, Marshal.ReadInt64(memory.Start, (int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 8, 0));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 7, 0));
                Assert.Equal(Memory.PageSize - 6, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 6, 0));
                Assert.Equal(Memory.PageSize - 5, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 5, 0));
                Assert.Equal(Memory.PageSize - 4, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 4, 0));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3, 0));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }
    }
}
