using Xunit;
using System;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Int64Load"/> instruction.
    /// </summary>
    public class Int64LoadTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(new LocalGet(), new Int64Load(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(578437695752307454, exports.Test(0));
                Assert.Equal(4397772758562636546, exports.Test(1));
                Assert.Equal(-2865124961678982141, exports.Test(2));
                Assert.Equal(3015227055211414788, exports.Test(3));
                Assert.Equal(-2582295154680986107, exports.Test(4));
                Assert.Equal(61970503589955334, exports.Test(5));
                Assert.Equal(242072279648263, exports.Test(6));
                Assert.Equal(945594842376, exports.Test(7));
                Assert.Equal(3693729853, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 7));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 6));
                Assert.Equal(Memory.PageSize - 6, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Int64Load"/> instruction.
        /// </summary>
        [Fact]
        public void Int64Load_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<long>.CreateInstance(
                new LocalGet(), new Int64Load { Offset = 1, }, new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal(4397772758562636546, exports.Test(0));
                Assert.Equal(-2865124961678982141, exports.Test(1));
                Assert.Equal(3015227055211414788, exports.Test(2));
                Assert.Equal(-2582295154680986107, exports.Test(3));
                Assert.Equal(61970503589955334, exports.Test(4));
                Assert.Equal(242072279648263, exports.Test(5));
                Assert.Equal(945594842376, exports.Test(6));
                Assert.Equal(3693729853, exports.Test(7));
                Assert.Equal(14428632, exports.Test(8));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 9));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 8));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 6));
                Assert.Equal(Memory.PageSize - 5, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 2));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(
                    () => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }
    }
}
