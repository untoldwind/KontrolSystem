﻿using Xunit;
using WebAssembly.Runtime;

using WebAssembly.Instructions;

namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="GlobalSet"/> instruction.
    /// </summary>
    public class GlobalSetTests {
        /// <summary>
        /// Framework for testing.
        /// </summary>
        public abstract class TestBase {
            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestInt32(int value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestInt64(long value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestFloat32(float value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestFloat64(double value);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="GlobalSet"/> instruction.
        /// </summary>
        [Fact]
        public void SetGlobal_Compiled() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Int32, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Int64, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Float32, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Float64, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Functions.Add(new Function { Type = 0, });
            module.Functions.Add(new Function { Type = 1, });
            module.Functions.Add(new Function { Type = 2, });
            module.Functions.Add(new Function { Type = 3, });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Int32,
                InitializerExpression = new Instruction[] { new Int32Constant(4), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Int64,
                InitializerExpression = new Instruction[] { new Int64Constant(5), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Float32,
                InitializerExpression = new Instruction[] { new Float32Constant(6), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Float64,
                InitializerExpression = new Instruction[] { new Float64Constant(7), new End(), },
            });
            module.Exports.Add(new Export { Index = 0, Name = nameof(TestBase.TestInt32) });
            module.Exports.Add(new Export { Index = 1, Name = nameof(TestBase.TestInt64) });
            module.Exports.Add(new Export { Index = 2, Name = nameof(TestBase.TestFloat32) });
            module.Exports.Add(new Export { Index = 3, Name = nameof(TestBase.TestFloat64) });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(0), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(1), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(2), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(3), new End(), },
            });

            var compiled = module.ToInstance<TestBase>();

            var exports = compiled.Exports;
            exports.TestInt32(4);
            exports.TestInt64(5);
            exports.TestFloat32(6);
            exports.TestFloat64(7);
        }

        /// <summary>
        /// Framework for testing.
        /// </summary>
        public abstract class RoundTripTestBase {
            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestInt32(int value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestInt64(long value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestFloat32(float value);

            /// <summary>
            /// Receives a value.
            /// </summary>
            public abstract void TestFloat64(double value);

            /// <summary>
            /// Returns a value.
            /// </summary>
            public abstract int TestInt32();

            /// <summary>
            /// Returns a value.
            /// </summary>
            public abstract long TestInt64();

            /// <summary>
            /// Returns a value.
            /// </summary>
            public abstract float TestFloat32();

            /// <summary>
            /// Returns a value.
            /// </summary>
            public abstract double TestFloat64();
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="GlobalSet"/> instruction by validating retention via the <see cref="GlobalGet"/> instruction.
        /// </summary>
        [Fact]
        public void SetGlobal_GetGlobal_Compiled() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Int32, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Int64, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Float32, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new[] { WebAssemblyValueType.Float64, },
                Returns = new WebAssemblyValueType[] { }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new WebAssemblyValueType[] { },
                Returns = new[] { WebAssemblyValueType.Int32, }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new WebAssemblyValueType[] { },
                Returns = new[] { WebAssemblyValueType.Int64, }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new WebAssemblyValueType[] { },
                Returns = new[] { WebAssemblyValueType.Float32, }
            });
            module.Types.Add(new WebAssemblyType {
                Parameters = new WebAssemblyValueType[] { },
                Returns = new[] { WebAssemblyValueType.Float64, }
            });
            for (uint i = 0; i <= 7; i++) {
                module.Functions.Add(new Function { Type = i, });
            }

            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Int32,
                InitializerExpression = new Instruction[] { new Int32Constant(0), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Int64,
                InitializerExpression = new Instruction[] { new Int64Constant(0), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Float32,
                InitializerExpression = new Instruction[] { new Float32Constant(0), new End(), },
            });
            module.Globals.Add(new Global {
                IsMutable = true,
                ContentType = WebAssemblyValueType.Float64,
                InitializerExpression = new Instruction[] { new Float64Constant(0), new End(), },
            });
            module.Exports.Add(new Export { Index = 0, Name = nameof(TestBase.TestInt32) });
            module.Exports.Add(new Export { Index = 1, Name = nameof(TestBase.TestInt64) });
            module.Exports.Add(new Export { Index = 2, Name = nameof(TestBase.TestFloat32) });
            module.Exports.Add(new Export { Index = 3, Name = nameof(TestBase.TestFloat64) });
            module.Exports.Add(new Export { Index = 4, Name = nameof(TestBase.TestInt32) });
            module.Exports.Add(new Export { Index = 5, Name = nameof(TestBase.TestInt64) });
            module.Exports.Add(new Export { Index = 6, Name = nameof(TestBase.TestFloat32) });
            module.Exports.Add(new Export { Index = 7, Name = nameof(TestBase.TestFloat64) });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(0), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(1), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(2), new End(), },
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(3), new End(), },
            });
            module.Codes.Add(new FunctionBody { Code = new Instruction[] { new GlobalGet(0), new End(), }, });
            module.Codes.Add(new FunctionBody { Code = new Instruction[] { new GlobalGet(1), new End(), }, });
            module.Codes.Add(new FunctionBody { Code = new Instruction[] { new GlobalGet(2), new End(), }, });
            module.Codes.Add(new FunctionBody { Code = new Instruction[] { new GlobalGet(3), new End(), }, });

            var compiled = module.ToInstance<RoundTripTestBase>();

            var exports = compiled.Exports;
            exports.TestInt32(4);
            exports.TestInt64(5);
            exports.TestFloat32(6);
            exports.TestFloat64(7);

            Assert.Equal(4, exports.TestInt32());
            Assert.Equal(5, exports.TestInt64());
            Assert.Equal(6, exports.TestFloat32());
            Assert.Equal(7, exports.TestFloat64());
        }

        /// <summary>
        /// Used by <see cref="SetGlobal_Imported_Compiled"/>.
        /// </summary>
        public static int MutableGlobal { get; set; }

        /// <summary>
        /// Tests that imported globals can be written.
        /// </summary>
        [Fact]
        public void SetGlobal_Imported_Compiled() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType { Parameters = new[] { WebAssemblyValueType.Int32, }, });
            module.Functions.Add(new Function { });
            module.Imports.Add(new Import.Global {
                Module = "Imported",
                Field = "Global",
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = true
            });
            module.Exports.Add(new Export { Name = nameof(CompilerTestBaseVoid<int>.Test) });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[] { new LocalGet(0), new GlobalSet(0), new End(), },
            });

            var compiled = module.ToInstance<CompilerTestBaseVoid<int>>(new ImportDictionary {
                { "Imported", "Global", new GlobalImport(() => MutableGlobal, value => MutableGlobal = value) },
            });


            compiled.Exports.Test(4);
            Assert.Equal(4, MutableGlobal);
        }
    }
}
