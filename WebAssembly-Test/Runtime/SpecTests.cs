using Xunit;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebAssembly.Test.Runtime {
    public class SpecTests {
        /// <summary>
        /// .NET 5 changed something about floating point processing that causes some spec tests to fail.
        /// </summary>
        static readonly bool IsNet5OrHigher = Environment.Version.Major >= 5;

        private readonly SpecTestRunner specTestRunner;
        public SpecTests(ITestOutputHelper testOutputHelper) {
            specTestRunner = new SpecTestRunner(testOutputHelper);
        }

        /// <summary>
        /// Runs the address tests.
        /// </summary>
        [Fact]
        public void SpecTest_address() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "address"), "address.json");
        }

        /// <summary>
        /// Runs the align tests.
        /// </summary>
        [Fact]
        public void SpecTest_align() {
            static bool skip(uint line) => line <= 454 || (line >= 807 && line <= 811) || (line >= 828 && line <= 850);
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "align"), "align.json", skip);
        }

        /// <summary>
        /// Runs the binary tests.
        /// </summary>
        [Fact]
        public void SpecTest_binary() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "binary"), "binary.json");
        }

        /// <summary>
        /// Runs the binary-leb128 tests.
        /// </summary>
        [Fact]
        public void SpecTest_binary_leb128() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "binary-leb128"), "binary-leb128.json");
        }

        /// <summary>
        /// Runs the block tests.
        /// </summary>
        [Fact]
        public void SpecTest_block() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "block"), "block.json");
        }

        /// <summary>
        /// Runs the br tests.
        /// </summary>
        [Fact]
        public void SpecTest_br() {
            var skips = new HashSet<uint>
                {
                // The JIT compiler encountered invalid IL code or an internal limitation.
                357, 361, 373, 374, 375, 378, 379, 382, 383, 384, 394, 396, 401, 406, 412, 415, 417,
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "br"), "br.json", skips.Contains);
        }

        /// <summary>
        /// Runs the br_if tests.
        /// </summary>
        [Fact]
        public void SpecTest_br_if() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "br_if"), "br_if.json",
                line =>
                (line >= 372 && line <= 478) // The JIT compiler encountered invalid IL code or an internal limitation.
            );
        }

        /// <summary>
        /// Runs the br_table tests.
        /// </summary>
        [Fact]
        public void SpecTest_br_table() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "br_table"), "br_table.json",
                line => line == 3 || // BranchTable requires all labels to have type Empty, but found Int32.
                (line >= 1247 && line <= 1426) || // has no method source.
                line == 1429 || // should have thrown an exception but did not.
                line == 1502// || // should have thrown an exception but did not.
            );
        }

        /// <summary>
        /// Runs the break-drop tests.
        /// </summary>
        [Fact]
        public void SpecTest_break_drop() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "break-drop"), "break-drop.json");
        }

        /// <summary>
        /// Runs the call tests.
        /// </summary>
        [Fact]
        public void SpecTest_call() {
            var skips = new HashSet<uint>
                {
                272, // Infinite loop
                273, // Infinite loop
                289, // IndexOutOfRangeException (expected to fail, but a better exception needed)
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "call"), "call.json", skips.Contains);
        }

        /// <summary>
        /// Runs the call_indirect tests.
        /// </summary>
        [Fact]
        public void SpecTest_call_indirect() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "call_indirect"), "call_indirect.json",
                line =>
                line == 556 || // Infinite loop
                (line >= 557 && line <= 589) || // No method source
                line == 940 // unknown function 0 doesn't have a test procedure set up.
            );
        }

        /// <summary>
        /// Runs the const tests.
        /// </summary>
        [Fact]
        public void SpecTest_const() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "const"), "const.json");
        }

        /// <summary>
        /// Runs the conversions tests.
        /// </summary>
        [Fact]
        public void SpecTest_conversions() {
            var skips = new HashSet<uint> { 88, 89, 93, 133, 134, 139, 183, 187, 229, 234, 236 };
            if (!Environment.Is64BitProcess) // 32-bit JIT operates differently as of .NET Core 3.1.
                skips.UnionWith(new uint[] { 454, 455, 470, 471 });
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "conversions"), "conversions.json", skips.Contains);
        }

        /// <summary>
        /// Runs the custom tests.
        /// </summary>
        [Fact]
        public void SpecTest_custom() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "custom"), "custom.json");
        }

        /// <summary>
        /// Runs the data tests.
        /// </summary>
        [Fact]
        public void SpecTest_data() {
            var skips = new HashSet<uint>
                {
                78, // MemoryAccessOutOfRangeException: Attempted to access 1 bytes of memory starting at offset 65536, which would have exceeded the allocated memory.
                83, // MemoryAccessOutOfRangeException: Attempted to access 1 bytes of memory starting at offset 65536, which would have exceeded the allocated memory.
                89, // MemoryAccessOutOfRangeException: Attempted to access 1 bytes of memory starting at offset 131072, which would have exceeded the allocated memory.
                94, // Attempted to access 1 bytes of memory starting at offset 0, which would have exceeded the allocated memory.
                103, // Attempted to access 1 bytes of memory starting at offset 0, which would have exceeded the allocated memory.
                108, // Attempted to access 1 bytes of memory starting at offset 65536, which would have exceeded the allocated memory.
                113, // Attempted to access 1 bytes of memory starting at offset 0, which would have exceeded the allocated memory.
                122, // Attempted to access 1 bytes of memory starting at offset 0, which would have exceeded the allocated memory.
                186, // No exception thrown. ModuleLoadException exception was expected.
                194, // No exception thrown. ModuleLoadException exception was expected.
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "data"), "data.json", skips.Contains);
        }

        /// <summary>
        /// Runs the elem tests.
        /// </summary>
        [Fact]
        public void SpecTest_elem() {
            var miscellaneous = new HashSet<uint>
                {
                170, 186, 203, // ModuleLoadException, MemoryAccessOutOfRangeException or OverflowException, but no exception was thrown.
                237, // number, when added to Length, would exceed the defined Maximum.
                318, // The delegate at position 9 is expected to be of type System.Action, but the supplied delegate is System.Func`1[System.Int32].
                357, 370, // Missing import for module1::shared-table.
            };

            var initializerIssues = new HashSet<uint>
                { // Initializer expression support for the Element section is limited to a single Int32 constant followed by end.
                53,
                60,
            };

            var exceptionExpected = new HashSet<uint>
                { // No exception thrown. ModuleLoadException exception was expected.
                143,
                152,
                161,
                178,
                195,
            };

            var failedLookUp = new HashSet<uint>
                { // Failed to look up method call-overwritten-element
                329,
            };

            var nullRef = new HashSet<uint>
                { // NullReferenceException
                353,
                366,
                379,
            };

            var notEqual = new HashSet<uint>
                {
                367, // Not equal: 68 and 65
                380, // Not equal: 69 and 65
                381, // Not equal: 70 and 66
            };

            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "elem"), "elem.json",
                line =>
                miscellaneous.Contains(line) ||
                initializerIssues.Contains(line) ||
                exceptionExpected.Contains(line) ||
                failedLookUp.Contains(line) ||
                nullRef.Contains(line) ||
                notEqual.Contains(line)
            );
        }

        /// <summary>
        /// Runs the endianness tests.
        /// </summary>
        [Fact]
        public void SpecTest_endianness() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "endianness"), "endianness.json");
        }

        /// <summary>
        /// Runs the exports tests.
        /// </summary>
        [Fact]
        public void SpecTest_exports() {
            var skips = new HashSet<uint>
                {
                33, // Exception expected but not thrown.
                37, // Exception expected but not thrown.
                41, // Exception expected but not thrown.
                45, // Exception expected but not thrown.
                49, // Exception expected but not thrown.
                82, // Exception expected but not thrown.
                86, // Exception expected but not thrown.
                90, // Exception expected but not thrown.
                94, // Exception expected but not thrown.
                98, // Exception expected but not thrown.
                130, // Exception expected but not thrown.
                139, // Exception expected but not thrown.
                143, // Exception expected but not thrown.
                147, // Exception expected but not thrown.
                179, // Exception expected but not thrown.
                188, // Exception expected but not thrown.
                192, // Exception expected but not thrown.
                196, // Exception expected but not thrown.
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "exports"), "exports.json", skips.Contains);
        }

        /// <summary>
        /// Runs the f32 tests.
        /// </summary>
        [Fact]
        public void SpecTest_f32() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f32"), "f32.json");
        }

        /// <summary>
        /// Runs the f32_bitwise tests.
        /// </summary>
        [Fact]
        public void SpecTest_f32_bitwise() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f32_bitwise"), "f32_bitwise.json");
        }

        /// <summary>
        /// Runs the f32_cmp tests.
        /// </summary>
        [Fact]
        public void SpecTest_f32_cmp() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f32_cmp"), "f32_cmp.json");
        }

        /// <summary>
        /// Runs the f64 tests.
        /// </summary>
        [Fact]
        public void SpecTest_f64() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f64"), "f64.json");
        }

        /// <summary>
        /// Runs the f64_bitwise tests.
        /// </summary>
        [Fact]
        public void SpecTest_f64_bitwise() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f64_bitwise"), "f64_bitwise.json");
        }

        /// <summary>
        /// Runs the f64_cmp tests.
        /// </summary>
        [Fact]
        public void SpecTest_f64_cmp() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "f64_cmp"), "f64_cmp.json");
        }

        /// <summary>
        /// Runs the fac tests.
        /// </summary>
        [Fact]
        public void SpecTest_fac() {
            var skips = new HashSet<uint>
                {
                89, // Infinite loop
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "fac"), "fac.json", skips.Contains);
        }

        /// <summary>
        /// Runs the float_exprs tests.
        /// </summary>
        [Fact]
        public void SpecTest_float_exprs() {
            var skips = new HashSet<uint>
                {
                511, // Arithmetic operation resulted in an overflow.
                519, // Arithmetic operation resulted in an overflow.
            };

            if (IsNet5OrHigher) {
                skips.Add(2351);
                skips.Add(2357);
            }

            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "float_exprs"), "float_exprs.json", skips.Contains);
        }

        /// <summary>
        /// Runs the float_literals tests.
        /// </summary>
        [Fact]
        public void SpecTest_float_literals() {
            var skips = new HashSet<uint>
                {
                109, // Not equal: 2141192192 and 2145386496
                111, // Not equal: 2139169605 and 2143363909
                112, // Not equal: 2142257232 and 2146451536
                113, // Not equal: -5587746 and -1393442
            };
            if (!Environment.Is64BitProcess) {
                skips.UnionWith(new uint[]
                {
                    141, // Not equal: 9219994337134247936 and 9222246136947933184
                    143, // Not equal: 9218888453225749180 and 9221140253039434428
                    144, // Not equal: 9219717281780008969 and 9221969081593694217
                    145, // Not equal: -3751748707474619 and -1499948893789371
                });
            }
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "float_literals"), "float_literals.json", skips.Contains);
        }

        /// <summary>
        /// Runs the float_memory tests.
        /// </summary>
        [Fact]
        public void SpecTest_float_memory() {
            var skips = new HashSet<uint>
                {
                21, // Not equal: 2141192192 and 2145386496
                73, // Not equal: 2141192192 and 2145386496
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "float_memory"), "float_memory.json", skips.Contains);
        }

        /// <summary>
        /// Runs the float_misc tests.
        /// </summary>
        [Fact]
        public void SpecTest_float_misc() {
            var skips = new HashSet<uint> { 662, };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "float_misc"), "float_misc.json", skips.Contains);
        }

        /// <summary>
        /// Runs the forward tests.
        /// </summary>
        [Fact]
        public void SpecTest_forward() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "forward"), "forward.json");
        }

        /// <summary>
        /// Runs the func tests.
        /// </summary>
        [Fact]
        public void SpecTest_func() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "func"), "func.json");
        }

        /// <summary>
        /// Runs the func_ptrs tests.
        /// </summary>
        [Fact]
        public void SpecTest_func_ptrs() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "func_ptrs"), "func_ptrs.json");
        }

        /// <summary>
        /// Runs the globals tests.
        /// </summary>
        [Fact]
        public void SpecTest_globals() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "globals"), "globals.json");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles -- Must match expectations of the target WASM.
        public abstract class IntegerMath<T> {
            public abstract T add(T x, T y);
            public abstract T sub(T x, T y);
            public abstract T mul(T x, T y);
            public abstract T div_s(T x, T y);
            public abstract T div_u(T x, T y);
            public abstract T rem_s(T x, T y);
            public abstract T rem_u(T x, T y);
            public abstract T and(T x, T y);
            public abstract T or(T x, T y);
            public abstract T xor(T x, T y);
            public abstract T shl(T x, T y);
            public abstract T shr_s(T x, T y);
            public abstract T shr_u(T x, T y);
            public abstract T rotl(T x, T y);
            public abstract T clz(T x);
            public abstract T ctz(T x);
            public abstract T popcnt(T x);
            public abstract int eqz(T x);
            public abstract int eq(T x, T y);
            public abstract int ne(T x, T y);
            public abstract int lt_s(T x, T y);
            public abstract int lt_u(T x, T y);
            public abstract int le_s(T x, T y);
            public abstract int le_u(T x, T y);
            public abstract int gt_s(T x, T y);
            public abstract int gt_u(T x, T y);
            public abstract int ge_s(T x, T y);
            public abstract int ge_u(T x, T y);
        }
#pragma warning restore

        /// <summary>
        /// Runs the i32 tests.
        /// </summary>
        [Fact]
        public void SpecTest_i32() {
            var skip = new HashSet<uint>
                {
               106, // Arithmetic operation resulted in an overflow.
            };
            specTestRunner.Run<IntegerMath<int>>(Path.Combine("Runtime", "SpecTestData", "i32"), "i32.json", skip.Contains);
        }

        /// <summary>
        /// Runs the i64 tests.
        /// </summary>
        [Fact]
        public void SpecTest_i64() {
            var skip = new HashSet<uint>
                {
               106, // Arithmetic operation resulted in an overflow.
            };
            specTestRunner.Run<IntegerMath<long>>(Path.Combine("Runtime", "SpecTestData", "i64"), "i64.json", skip.Contains);
        }

        /// <summary>
        /// Runs the if tests.
        /// </summary>
        [Fact]
        public void SpecTest_if() {
            var skip = new HashSet<uint>
                {
                491, // Unreachable instruction was encountered.
                492, // The JIT compiler encountered invalid IL code or an internal limitation.
                493, // The JIT compiler encountered invalid IL code or an internal limitation.
                495, // Not equal: -14 and - 231.
                564, // Should have thrown an exception but did not.
                576, // Should have thrown an exception but did not.
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "if"), "if.json", skip.Contains);
        }

        /// <summary>
        /// Runs the imports tests.
        /// </summary>
        [Fact]
        public void SpecTest_imports() {
            var skip = new HashSet<uint>
                {
                322, // Missing import for test::table-10-inf.
                323, // Missing import for test::table-10-inf.
                324, // Missing import for test::table-10-inf.
                352, // ImportException exception was expected
                356, // ImportException exception was expected
                405, // ModuleLoadException exception was expected.
                417, // Missing import for test::memory-2-inf.
                418, // Missing import for test::memory-2-inf.
                419, // Missing import for test::memory-2-inf.
                445, // ImportException exception was expected.
                449, // ImportException exception was expected.
                479, // ImportException exception was expected.
                483, // ImportException exception was expected.
                283,
                286,
                302,
                305,
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "imports"), "imports.json", skip.Contains);
        }

        /// <summary>
        /// Runs the int_exprs tests.
        /// </summary>
        [Fact]
        public void SpecTest_int_exprs() {
            HashSet<uint>? skips = null;
            if (!Environment.Is64BitProcess) {
                skips = new HashSet<uint>
                    {
                    58, 59, 77, 78,
                };
            }
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "int_exprs"), "int_exprs.json", skips != null ? (Func<uint, bool>)skips.Contains : null);
        }

        /// <summary>
        /// Runs the int_literals tests.
        /// </summary>
        [Fact]
        public void SpecTest_int_literals() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "int_literals"), "int_literals.json");
        }

        /// <summary>
        /// Runs the labels tests.
        /// </summary>
        [Fact(Skip = "StackTooSmallException")]
        public void SpecTest_labels() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "labels"), "labels.json");
        }

        /// <summary>
        /// Runs the left-to-right tests.
        /// </summary>
        [Fact]
        public void SpecTest_left_to_right() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "left-to-right"), "left-to-right.json");
        }

        /// <summary>
        /// Runs the linking tests.
        /// </summary>
        [Fact]
        public void SpecTest_linking() {
            var skips = new HashSet<uint>
                {
                48, // setter cannot have a return type.
                50, //  Missing import for Mg::glob.
                68, 69, 71, 72, 75, 77, 81, 83, // The given key '$Ng' was not present in the dictionary.
                154, // Missing import for Mt::tab.
                170, // The given key '$Ot' was not present in the dictionary.
                172, 173, 175, // Not equal: -4 and 4
                176, // The given key '$Ot' was not present in the dictionary.
                178, 179, 181,// Object reference not set to an instance of an object.
                182, // The given key '$Ot' was not present in the dictionary.
                192, // Missing import for Mt::tab.
                204, // The given key '$G2' was not present in the dictionary.
                207, 228, 239, // Missing import for Mt::tab.
                279, // Missing import for Mm::mem.
                288, 289, // Not equal: 167 and 2
                291, // The given key '$Om' was not present in the dictionary.
                293, 299, 306, // Missing import for Mm::mem.
                314, 315, 316, 317, 318, 319, 320, 321, // The given key '$Pm' was not present in the dictionary.
                335, 345, // Missing import for Mm::mem.
                371, // ModuleLoadException exception was expected.
                387, // Not equal: 104 and 0
                388, // Object reference not set to an instance of an object.
                136,
                137,
                139,
                141,
                142,
                144,
                184,
                185,
                187,
                225,
                236,
                248,
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "linking"), "linking.json", skips.Contains);
        }

        /// <summary>
        /// Runs the load tests.
        /// </summary>
        [Fact]
        public void SpecTest_load() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "load"), "load.json");
        }

        /// <summary>
        /// Runs the local_get tests.
        /// </summary>
        [Fact]
        public void SpecTest_local_get() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "local_get"), "local_get.json");
        }

        /// <summary>
        /// Runs the local_set tests.
        /// </summary>
        [Fact]
        public void SpecTest_local_set() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "local_set"), "local_set.json");
        }

        /// <summary>
        /// Runs the local_tee tests.
        /// </summary>
        [Fact]
        public void SpecTest_local_tee() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "local_tee"), "local_tee.json");
        }

        /// <summary>
        /// Runs the loop tests.
        /// </summary>
        [Fact]
        public void SpecTest_loop() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "loop"), "loop.json");
        }

        /// <summary>
        /// Runs the memory tests.
        /// </summary>
        [Fact]
        public void SpecTest_memory() {
            var skip = new HashSet<uint>
                {
                49, // No exception thrown. ModuleLoadException exception was expected.
                53, // No exception thrown. ModuleLoadException exception was expected.
                57, // No exception thrown. ModuleLoadException exception was expected.
                61, // No exception thrown. ModuleLoadException exception was expected.
                65, // No exception thrown. ModuleLoadException exception was expected.
                69, // No exception thrown. ModuleLoadException exception was expected.
                73, // No exception thrown. ModuleLoadException exception was expected.
            };
            if (!Environment.Is64BitProcess)
                skip.UnionWith(Enumerable.Range(187, 26).Select(i => (uint)i)); // Common Language Runtime detected an invalid program.
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "memory"), "memory.json", skip.Contains);
        }

        /// <summary>
        /// Runs the memory_grow tests.
        /// </summary>
        [Fact]
        public void SpecTest_memory_grow() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "memory_grow"), "memory_grow.json");
        }

        /// <summary>
        /// Runs the memory_redundancy tests.
        /// </summary>
        [Fact]
        public void SpecTest_memory_redundancy() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "memory_redundancy"), "memory_redundancy.json");
        }

        /// <summary>
        /// Runs the memory_size tests.
        /// </summary>
        [Fact]
        public void SpecTest_memory_size() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "memory_size"), "memory_size.json");
        }

        /// <summary>
        /// Runs the names tests.
        /// </summary>
        [Fact]
        public void SpecTest_names() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "names"), "names.json");
        }

        /// <summary>
        /// Runs the nop tests.
        /// </summary>
        [Fact]
        public void SpecTest_nop() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "nop"), "nop.json");
        }

        /// <summary>
        /// Runs the return tests.
        /// </summary>
        [Fact]
        public void SpecTest_return() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "return"), "return.json");
        }

        /// <summary>
        /// Runs the select tests.
        /// </summary>
        [Fact]
        public void SpecTest_select() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "select"), "select.json");
        }

        /// <summary>
        /// Runs the skip-stack-guard-page tests.
        /// </summary>
        [Fact(Skip = "Causes CLR malfunction.")]
        public void SpecTest_skip_stack_guard_page() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "skip-stack-guard-page"), "skip-stack-guard-page.json");
        }

        /// <summary>
        /// Runs the stack tests.
        /// </summary>
        [Fact]
        public void SpecTest_stack() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "stack"), "stack.json");
        }

        /// <summary>
        /// Runs the store tests.
        /// </summary>
        [Fact]
        public void SpecTest_store() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "store"), "store.json");
        }

        /// <summary>
        /// Runs the switch tests.
        /// </summary>
        [Fact]
        public void SpecTest_switch() {
            var skips = new HashSet<uint>
                {
                138, // JIT Compiler encountered an internal limitation.
                139, // JIT Compiler encountered an internal limitation.
                140, // JIT Compiler encountered an internal limitation.
                141, // JIT Compiler encountered an internal limitation.
                142, // JIT Compiler encountered an internal limitation.
                143, // JIT Compiler encountered an internal limitation.
                144, // JIT Compiler encountered an internal limitation.
                145, // JIT Compiler encountered an internal limitation.
                146, // JIT Compiler encountered an internal limitation.
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "switch"), "switch.json", skips.Contains);
        }

        /// <summary>
        /// Runs the traps tests.
        /// </summary>
        [Fact]
        public void SpecTest_traps() {
            var skips = new HashSet<uint>
                {
                83, // threw an unexpected exception of type InvalidProgramException.
                91, // threw an unexpected exception of type InvalidProgramException.
            };
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "traps"), "traps.json", skips.Contains);
        }

        /// <summary>
        /// Runs the type tests.
        /// </summary>
        [Fact]
        public void SpecTest_type() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "type"), "type.json");
        }

        /// <summary>
        /// Runs the unreachable tests.
        /// </summary>
        [Fact]
        public void SpecTest_unreachable() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "unreachable"), "unreachable.json");
        }

        /// <summary>
        /// Runs the unreached-invalid tests.
        /// </summary>
        [Fact]
        public void SpecTest_unreached_invalid() {
            var skips = new HashSet<uint>
                {
                490, // should have thrown an exception but did not.
                585, // should have thrown an exception but did not.
                604, // should have thrown an exception but did not.
                676, // should have thrown an exception but did not.
                690, // should have thrown an exception but did not.
            };

            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "unreached-invalid"), "unreached-invalid.json", skips.Contains);
        }

        /// <summary>
        /// Runs the unwind tests.
        /// </summary>
        [Fact(Skip = "The JIT compiler encountered invalid IL code or an internal limitation.")]
        public void SpecTest_unwind() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "unwind"), "unwind.json");
        }

        /// <summary>
        /// Runs the utf8-custom-section-id tests.
        /// </summary>
        [Fact]
        public void SpecTest_utf8_custom_section_id() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "utf8-custom-section-id"), "utf8-custom-section-id.json");
        }

        /// <summary>
        /// Runs the utf8-import-field tests.
        /// </summary>
        [Fact]
        public void SpecTest_utf8_import_field() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "utf8-import-field"), "utf8-import-field.json");
        }

        /// <summary>
        /// Runs the utf8-import-module tests.
        /// </summary>
        [Fact]
        public void SpecTest_utf8_import_module() {
            specTestRunner.Run(Path.Combine("Runtime", "SpecTestData", "utf8-import-module"), "utf8-import-module.json");
        }
    }
}
