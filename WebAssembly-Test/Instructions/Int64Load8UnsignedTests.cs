using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Load8Unsigned"/> instruction.
    /// </summary>
    public class Int64Load8UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load8Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load8Unsigned_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(new LocalGet(), new Int64Load8Unsigned(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(254, exports.Test(0));
                Assert.Equal(2, exports.Test(1));
                Assert.Equal(3, exports.Test(2));
                Assert.Equal(4, exports.Test(3));
                Assert.Equal(5, exports.Test(4));
                Assert.Equal(6, exports.Test(5));
                Assert.Equal(7, exports.Test(6));
                Assert.Equal(8, exports.Test(7));
                Assert.Equal(61, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 4));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(1u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load8Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load8Unsigned_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(
                new LocalGet(), new Int64Load8Unsigned { Offset = 1, }, new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(2, exports.Test(0));
                Assert.Equal(3, exports.Test(1));
                Assert.Equal(4, exports.Test(2));
                Assert.Equal(5, exports.Test(3));
                Assert.Equal(6, exports.Test(4));
                Assert.Equal(7, exports.Test(5));
                Assert.Equal(8, exports.Test(6));
                Assert.Equal(61, exports.Test(7));
                Assert.Equal(216, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 5));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(1u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load8Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load8Unsigned_Compiled_Then_Shift() {
            if (!Environment.Is64BitProcess)
                Assert.True(false, "32-bit .NET doesn't support 64-bit bit shift amounts.");

            // seems like the offset here needs to be different so it
            // doesn't interfere with other tests above.  so I just
            // picked a number.
            const int off = 4;

            const byte b = 0x9f;

            // shift needs to be >23 to repro the problem
            const int shift = 40;

            // the issue here is that Ldind_U8 results in int32,
            // not int64.  so if the shift would push things too
            // far, it gives the wrong result.

            // Int64Load8Unsigned will need to do a Conv_I8

            // in fact, I believe this affects Int64LoadFoo, for 
            // all Foo narrow than 64 bits

            var compiled = MemoryReadTestBase<long>.CreateInstance(new LocalGet(),
                new Int64Load8Unsigned { Offset = off, }, new Int64Constant(shift), new Int64ShiftLeft(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;

                Marshal.WriteByte(memory.Start + off, b);
                var should_be = ((long)b) << shift;
                Assert.Equal(should_be, exports.Test(0));
            }
        }

    }
}
