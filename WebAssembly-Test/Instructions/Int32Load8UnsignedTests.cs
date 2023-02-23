using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int32Load8Unsigned"/> instruction.
    /// </summary>
    public class Int32Load8UnsignedTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int32Load8Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Load8Unsigned_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<int>.CreateInstance(new LocalGet(), new Int32Load8Unsigned(), new End());

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
        /// Tests compilation and execution of the <see cref="Int32Load8Unsigned"/> instruction.
        /// </summary>
        [Fact]
        public void Int32Load8Unsigned_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<int>.CreateInstance(
                new LocalGet(), new Int32Load8Unsigned { Offset = 1, }, new End());

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
    }
}
