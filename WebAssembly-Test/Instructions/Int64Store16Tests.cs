﻿using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Store16"/> instruction.
    /// </summary>
    public class Int64Store16Tests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Store16"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Store16_Compiled_Offset0() {
            if (!Environment.Is64BitProcess) Assert.True(false, "32-bit .NET has an unknown error with this process.");

            var compiled = MemoryWriteTestBase<long>.CreateInstance(
                new LocalGet(0), new LocalGet(1), new Int64Store16(), new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, 32768);
                Assert.Equal(32768, Marshal.ReadInt32(memory.Start));
                Assert.Equal(128, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 2));

                exports.Test((int)Memory.PageSize - 8, 1);

                Assert.Equal(1, Marshal.ReadInt64(memory.Start, (int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(2u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(2u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Store16"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Store16_Compiled_Offset1() {
            if (!Environment.Is64BitProcess) Assert.True(false, "32-bit .NET has an unknown error with this process.");

            var compiled = MemoryWriteTestBase<long>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Int64Store16() { Offset = 1 }, new End());
            Assert.NotNull(compiled);

            using (compiled) {
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                exports.Test(0, 32768);
                Assert.Equal(8388608, Marshal.ReadInt32(memory.Start));
                Assert.Equal(32768, Marshal.ReadInt32(memory.Start, 1));
                Assert.Equal(128, Marshal.ReadInt32(memory.Start, 2));
                Assert.Equal(0, Marshal.ReadInt32(memory.Start, 3));

                exports.Test((int)Memory.PageSize - 8 - 1, 1);

                Assert.Equal(1, Marshal.ReadInt64(memory.Start, (int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize - 2, 0));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(2u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() =>
                    exports.Test((int)Memory.PageSize - 1, 0));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(2u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue), 0));
            }
        }
    }
}
