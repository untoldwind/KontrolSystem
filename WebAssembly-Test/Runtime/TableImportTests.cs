using Xunit;
using WebAssembly.Instructions;
using WebAssembly.Runtime;
using System;
using System.Reflection;

namespace WebAssembly.Test.Runtime {
    public class TableImportTests {
        /// <summary>
        /// Tests adding a function delegate to an imported table.
        /// </summary>
        [Fact]
        public void Compile_TableImport_AddFunction() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[] { WebAssemblyValueType.Int32 },
                Parameters = new[] { WebAssemblyValueType.Int32 }
            });
            module.Imports.Add(new Import.Table("Test", "Test", 1));
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = "Test",
            });
            module.Elements.Add(new Element(0, 0));
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new LocalGet(0),
                    new End()
                },
            });

            var table = new FunctionTable(1);
            Assert.Equal(1u, table.Length);
            Assert.Null(table[0]);

            var compiled = module.ToInstance<CompilerTestBase<int>>(
                new ImportDictionary {
                    { "Test", "Test", table },
                });

            var rawDelegate = table[0];
            Assert.NotNull(rawDelegate);
            Assert.IsAssignableFrom<Func<int, int>>(rawDelegate);
            var nativeDelegate = (Func<int, int>)rawDelegate!;
            Assert.Equal(0, nativeDelegate(0));
            Assert.Equal(5, nativeDelegate(5));
        }

        /// <summary>
        /// Tests a function delegate already present on an imported table.
        /// </summary>
        [Fact]
        public void Compile_TableImport_ExistingFunction() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[] { WebAssemblyValueType.Int32 },
                Parameters = new[] { WebAssemblyValueType.Int32 }
            });
            module.Imports.Add(new Import.Table("Test", "Test", 1));
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = "Test",
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new LocalGet(0),
                    new Int32Constant(0),
                    new CallIndirect(0),
                    new End()
                },
            });

            var table = new FunctionTable(1);
            var calls = 0;
            table[0] = new Func<int, int>(value => { calls++; return value + 1; });

            var compiled = module.ToInstance<CompilerTestBase<int>>(
                new ImportDictionary {
                    { "Test", "Test", table },
                });

            Assert.Equal(0, calls);
            Assert.Equal(3, compiled.Exports.Test(2));
            Assert.Equal(1, calls);
        }

        /// <summary>
        /// Runs the sample WASM from https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/WebAssembly/Table .
        /// </summary>
        [Fact]
        public void Execute_Sample_MDN_Table2() {
            var tbl = new FunctionTable(2);
            Assert.Equal(2u, tbl.Length);
            Assert.Null(tbl[0]);
            Assert.Null(tbl[1]);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebAssembly.Test.Samples.table2.wasm")) {
                var imports = new ImportDictionary
                    {
                    { "js", "tbl", tbl },
                };
                Assert.NotNull(stream);
                Compile.FromBinary<dynamic>(stream!)(imports);
            }

            Assert.Equal(2u, tbl.Length);

            var f1 = tbl[0];
            Assert.NotNull(f1);
            Assert.IsAssignableFrom<Func<int>>(f1);
            Assert.Equal(42, ((Func<int>)f1!).Invoke());

            var f2 = tbl[1];
            Assert.NotNull(f2);
            Assert.IsAssignableFrom<Func<int>>(f1);
            Assert.Equal(83, ((Func<int>)f2!).Invoke());
        }

        /// <summary>
        /// Used to test table export functionality via tests like <see cref="Compile_TableImport_ExportedButNotUsedInternally"/>.
        /// </summary>
        public abstract class ExportedTable {
            /// <summary>
            /// An exported table.
            /// </summary>
            public abstract FunctionTable Table { get; }
        }

        /// <summary>
        /// Tests exporting a function table that wasn't imported or defined.
        /// </summary>
        [Fact]
        public void Compile_TableImport_ExportedButNotUsedInternally() {
            var module = new Module();
            module.Exports.Add(new Export {
                Name = nameof(ExportedTable.Table),
                Kind = ExternalKind.Table,
            });

            Assert.Throws<ModuleLoadException>(() => module.ToInstance<ExportedTable>(new ImportDictionary()));
        }

        /// <summary>
        /// Tests exporting a function table that was imported.
        /// </summary>
        [Fact]
        public void Compile_TableImport_ExportedImport() {
            var module = new Module();
            module.Imports.Add(new Import.Table("Test", "Test", 1));
            module.Exports.Add(new Export {
                Name = nameof(ExportedTable.Table),
                Kind = ExternalKind.Table,
            });

            var table = new FunctionTable(0);

            var exportedTable = module.ToInstance<ExportedTable>(
                new ImportDictionary {
                    { "Test", "Test", table },
                })
                .Exports
                .Table;

            Assert.Same(table, exportedTable);
        }

        /// <summary>
        /// Extends <see cref="ExportedTable"/> with a test method.
        /// </summary>
        public abstract class ExportedTableWithTest : ExportedTable {
            /// <summary>
            /// Runs an exported test method.
            /// </summary>
            public abstract int Test(int value);
        }

        /// <summary>
        /// Extends <see cref="ExportedTable"/> with a <see cref="Calls"/> property.
        /// </summary>
        public abstract class ExportedTableWithCalls : ExportedTable {
            /// <summary>
            /// A counter for calls.
            /// </summary>
            public abstract int Calls { get; }
        }

        /// <summary>
        /// Tests multiple assemblies connected via a shared table.
        /// </summary>
        [Fact]
        public void Compile_TableImport_MultiAssemblySharedTable() {
            var module1 = new Module();
            module1.Types.Add(new WebAssemblyType {
                Returns = new[] { WebAssemblyValueType.Int32 },
                Parameters = new[] { WebAssemblyValueType.Int32 }
            });
            module1.Exports.Add(new Export {
                Name = nameof(ExportedTableWithCalls.Table),
                Kind = ExternalKind.Table,
            });
            module1.Exports.Add(new Export {
                Name = nameof(ExportedTableWithCalls.Calls),
                Kind = ExternalKind.Global,
            });
            module1.Globals.Add(new Global {
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = true,
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(0),
                    new End()
                }
            });
            module1.Functions.Add(new Function {
            });
            module1.Tables.Add(new Table(2));
            module1.Elements.Add(new Element(0, 0));
            module1.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new GlobalGet(0),
                    new Int32Constant(1),
                    new Int32Add(),
                    new GlobalSet(0),

                    new LocalGet(0),
                    new End()
                },
            });

            var exports1 = module1.ToInstance<ExportedTableWithCalls>().Exports;
            var sharedTable = exports1.Table;
            Assert.NotNull(sharedTable);
            var del0 = sharedTable[0];
            Assert.NotNull(del0);
            Assert.IsAssignableFrom<Func<int, int>>(del0);
            var func0 = (Func<int, int>)del0!;
            Assert.Equal(0, exports1.Calls);
            Assert.Equal(5, func0(5));
            Assert.Equal(1, exports1.Calls);

            var module2 = new Module {
                Types = module1.Types,
            };
            module2.Imports.Add(new Import.Table("Test", "Test", 1));
            module2.Functions.Add(new Function {
            });
            module2.Exports.Add(new Export {
                Name = "Test",
            });
            module2.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new LocalGet(0),
                    new Int32Constant(0),
                    new CallIndirect(0),
                    new End()
                },
            });

            var compiled = module2.ToInstance<CompilerTestBase<int>>(
                new ImportDictionary {
                    { "Test", "Test", sharedTable },
                });

            Assert.Equal(1, exports1.Calls);
            Assert.Equal(3, compiled.Exports.Test(3));
            Assert.Equal(2, exports1.Calls);
        }

        /// <summary>
        /// Test importing a table whose initial size is too small.
        /// </summary>
        [Fact]
        public void Compile_TableImport_UndersizedTable() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[] { WebAssemblyValueType.Int32 },
                Parameters = new[] { WebAssemblyValueType.Int32 }
            });
            module.Imports.Add(new Import.Table("Test", "Test", 1));
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = "Test",
            });
            module.Elements.Add(new Element(0, 0));
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new LocalGet(0),
                    new End()
                },
            });

            var table = new FunctionTable(0, 1);
            Assert.Equal(0u, table.Length);

            var compiled = module.ToInstance<CompilerTestBase<int>>(
                new ImportDictionary {
                    { "Test", "Test", table },
                });

            Assert.Equal(1u, table.Length);
            var rawDelegate = table[0];
            Assert.NotNull(rawDelegate);
            Assert.IsAssignableFrom<Func<int, int>>(rawDelegate);
            var nativeDelegate = (Func<int, int>)rawDelegate!;
            Assert.Equal(0, nativeDelegate(0));
            Assert.Equal(5, nativeDelegate(5));
        }

        /// <summary>
        /// Verifies that <see cref="Table"/> is initialized with a correct element type.
        /// </summary>
        [Fact]
        public void ImportTable_InitialElementTypeIsCorrect() {
            Assert.Equal(ElementType.FunctionReference, new Table().ElementType);
        }
    }

}
