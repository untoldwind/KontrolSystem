using Xunit;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using WebAssembly.Runtime;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float32Load"/> instruction.
    /// </summary>
    public class Float32LoadTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Load"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Load_Compiled_Offset0() {
            var compiled = MemoryReadTestBase<float>.CreateInstance(new LocalGet(), new Float32Load(), new End());

            using (compiled) {
                Assert.NotNull(compiled);

                var exports = compiled.Exports;
                Assert.NotNull(exports);
                var memory = exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);
                Assert.Equal(0, exports.Test(0));

                var invariantCulture = CultureInfo.InvariantCulture;

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal("1.540035E-36", exports.Test(0).ToString(invariantCulture));
                Assert.Equal("6.207163E-36", exports.Test(1).ToString(invariantCulture));
                Assert.Equal("2.501747E-35", exports.Test(2).ToString(invariantCulture));
                Assert.Equal("1.008251E-34", exports.Test(3).ToString(invariantCulture));
                Assert.Equal("4.063216E-34", exports.Test(4).ToString(invariantCulture));
                Assert.Equal("0.03320982", exports.Test(5).ToString(invariantCulture));
                Assert.Equal("-8.313687E+14", exports.Test(6).ToString(invariantCulture));
                Assert.Equal("9.602914E-14", exports.Test(7).ToString(invariantCulture));
                Assert.Equal("-1.912281E+17", exports.Test(8).ToString(invariantCulture));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 4));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(4u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Load"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Load_Compiled_Offset1() {
            var compiled = MemoryReadTestBase<float>.CreateInstance(
                new LocalGet(), new Float32Load { Offset = 1, }, new End());

            using (compiled) {
                Assert.NotNull(compiled);

                var exports = compiled.Exports;
                Assert.NotNull(exports);
                var memory = exports.Memory;
                Assert.NotEqual(IntPtr.Zero, memory.Start);
                Assert.Equal(0, exports.Test(0));

                var invariantCulture = CultureInfo.InvariantCulture;

                var testData = Samples.Memory;
                Marshal.Copy(testData, 0, memory.Start, testData.Length);
                Assert.Equal("6.207163E-36", exports.Test(0).ToString(invariantCulture));
                Assert.Equal("2.501747E-35", exports.Test(1).ToString(invariantCulture));
                Assert.Equal("1.008251E-34", exports.Test(2).ToString(invariantCulture));
                Assert.Equal("4.063216E-34", exports.Test(3).ToString(invariantCulture));
                Assert.Equal("0.03320982", exports.Test(4).ToString(invariantCulture));
                Assert.Equal("-8.313687E+14", exports.Test(5).ToString(invariantCulture));
                Assert.Equal("9.602914E-14", exports.Test(6).ToString(invariantCulture));
                Assert.Equal("-1.912281E+17", exports.Test(7).ToString(invariantCulture));
                Assert.Equal("2.021882E-38", exports.Test(8).ToString(invariantCulture));

                Assert.Equal(0, exports.Test((int)Memory.PageSize - 5));

                MemoryAccessOutOfRangeException x;

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 4));
                Assert.Equal(Memory.PageSize - 3, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 3));
                Assert.Equal(Memory.PageSize - 2, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 2));
                Assert.Equal(Memory.PageSize - 1, x.Offset);
                Assert.Equal(4u, x.Length);

                x = Assert.Throws<MemoryAccessOutOfRangeException>(() => exports.Test((int)Memory.PageSize - 1));
                Assert.Equal(Memory.PageSize, x.Offset);
                Assert.Equal(4u, x.Length);

                Assert.Throws<OverflowException>(() => exports.Test(unchecked((int)uint.MaxValue)));
            }
        }

        /// <summary>
        /// Tests <see cref="Float32Load.Equals(Float32Load)"/>
        /// </summary>
        [Fact]
        public void Float32Load_Equals() {
            TestUtility.CreateInstances<Float32Load>(out var a, out var b);

            a.Flags = MemoryImmediateInstruction.Options.Align2;
            TestUtility.AreNotEqual(a, b);
            b.Flags = MemoryImmediateInstruction.Options.Align2;
            TestUtility.AreEqual(a, b);

            a.Offset = 1;
            TestUtility.AreNotEqual(a, b);
            b.Offset = 1;
            TestUtility.AreEqual(a, b);
        }
    }
}
