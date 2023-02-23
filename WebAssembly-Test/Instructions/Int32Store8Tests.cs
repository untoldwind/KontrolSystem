using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32Store8"/> instruction.
    /// </summary>
    public class Int32Store8Tests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Store8"/> instruction.
        /// </summary>
        [Fact]
        public void IInt32Store8_Compiled_Offset0() {
            var compiled = MemoryWriteTestBase<int>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int32Store8(), new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, 128);
                Assert.Equal(128, Marshal.ReadInt32(memory.Start));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 3));

                exports.Test((int)Memory.PageSize - 1, 1);

                Assert.Equal(1, Marshal.ReadByte(memory.Start, (int)Memory.PageSize - 1));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(1u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Store8"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Store8_Compiled_Offset1() {
            var compiled = MemoryWriteTestBase<int>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Int32Store8() { Offset = 1 }, new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, 128);
                Assert.Equal(32768, Marshal.ReadInt32(memory.Start));
                Assert.Equal(128, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 3));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 4));

                exports.Test((int)Memory.PageSize - 1 - 1, 1);

                Assert.Equal(1, Marshal.ReadByte(memory.Start, (int)Memory.PageSize - 1));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(1u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }
    }
}
