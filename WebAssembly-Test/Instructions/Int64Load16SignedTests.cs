using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Load16Signed"/> instruction.
    /// </summary>
    public class Int64Load16SignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load16Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load16Signed_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(new LocalGet(), new Int64Load16Signed(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(766, exports.Test(0));
                Assert.Equal(770, exports.Test(1));
                Assert.Equal(1027, exports.Test(2));
                Assert.Equal(1284, exports.Test(3));
                Assert.Equal(1541, exports.Test(4));
                Assert.Equal(1798, exports.Test(5));
                Assert.Equal(2055, exports.Test(6));
                Assert.Equal(15624, exports.Test(7));
                Assert.Equal(-10179, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 2));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(2u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(2u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load16Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load16Signed_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(
                new LocalGet(), new Int64Load16Signed { Offset = 1, }, new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(770, exports.Test(0));
                Assert.Equal(1027, exports.Test(1));
                Assert.Equal(1284, exports.Test(2));
                Assert.Equal(1541, exports.Test(3));
                Assert.Equal(1798, exports.Test(4));
                Assert.Equal(2055, exports.Test(5));
                Assert.Equal(15624, exports.Test(6));
                Assert.Equal(-10179, exports.Test(7));
                Assert.Equal(10712, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 3));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 2));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(2u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(2u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load8Signed"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load16Signed_Compiled_Then_Shift() {
            if (!Environment.Is64BitProcess)
                Assert.True(false, "32-bit .NET doesn't support 64-bit bit shift amounts.");

            // Adapted from Int64Load8Unsigned_Compiled_Then_Shift.
            const int off = 4;
            const sbyte b = 0x5f;
            const int shift = 40;

            var compiled = MemoryReadTestBase<long>.CreateInstance(new LocalGet(),
                new Int64Load16Signed { Offset = off, }, new Int64Constant(shift), new Int64ShiftLeft(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;

                Marshal.WriteByte(memory.Start + off, (byte)b);
                const long should_be = ((long)b) << shift;
                Assert.Equal(should_be, exports.Test(0));
            }
        }
    }
}
