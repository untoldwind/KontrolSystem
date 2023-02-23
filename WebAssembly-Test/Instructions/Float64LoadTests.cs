using Xunit;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64Load"/> instruction.
    /// </summary>
    public class Float64LoadTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Load"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Load_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<double>.CreateInstance(new LocalGet(), new Float64Load(), new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var invariantCulture = CultureInfo.InvariantCulture;

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal("5.44760372201182E-270", exports.Test(0).ToString(invariantCulture));
                Assert.Equal("1.06703248910785E-14", exports.Test(1).ToString(invariantCulture));
                Assert.Equal("-1.14389371511465E+117", exports.Test(2).ToString(invariantCulture));
                Assert.Equal("4.12824598825351E-107", exports.Test(3).ToString(invariantCulture));
                Assert.Equal("-9.39245758009613E+135", exports.Test(4).ToString(invariantCulture));
                Assert.Equal("1.60424369241791E-304", exports.Test(5).ToString(invariantCulture));
                Assert.Equal("1.19599597184682E-309", exports.Test(6).ToString(invariantCulture));
                Assert.Equal("4.6718592650265E-312", exports.Test(7).ToString(invariantCulture));
                Assert.Equal("1.82494502538554E-314", exports.Test(8).ToString(invariantCulture));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 8));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 7));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 6));
                Assert.Equal(Memory.PageSize - 6, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64Load"/> instruction.
        /// </summary>
        [Fact]
        public void Float64Load_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<double>.CreateInstance(
                new LocalGet(), new Float64Load { Offset = 1, }, new End());

            using (compiled) {
                Assert.NotNull(compiled);
                Assert.NotNull(compiled.Exports);
                var memory = compiled.Exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);

                var exports = compiled.Exports;
                Assert.Equal(0, exports.Test(0));

                var invariantCulture = CultureInfo.InvariantCulture;

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal("1.06703248910785E-14", exports.Test(0).ToString(invariantCulture));
                Assert.Equal("-1.14389371511465E+117", exports.Test(1).ToString(invariantCulture));
                Assert.Equal("4.12824598825351E-107", exports.Test(2).ToString(invariantCulture));
                Assert.Equal("-9.39245758009613E+135", exports.Test(3).ToString(invariantCulture));
                Assert.Equal("1.60424369241791E-304", exports.Test(4).ToString(invariantCulture));
                Assert.Equal("1.19599597184682E-309", exports.Test(5).ToString(invariantCulture));
                Assert.Equal("4.6718592650265E-312", exports.Test(6).ToString(invariantCulture));
                Assert.Equal("1.82494502538554E-314", exports.Test(7).ToString(invariantCulture));
                Assert.Equal("7.12869138768568E-317", exports.Test(8).ToString(invariantCulture));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 9));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 8));
                Assert.Equal(Memory.PageSize - 7, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 6));
                Assert.Equal(Memory.PageSize - 5, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(8u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(8u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }
    }
}
