using Xunit;
using WebAssembly.Runtime;

namespace WebAssembly.Test {
    public class GlobalImportTests {
        /// <summary>
        /// Always returns 3, used by <see cref="Compile_GlobalImmutableImportExport"/>.
        /// </summary>
        public static int ImportedImmutableGlobalReturns3 => 3;

        /// <summary>
        /// Verifies that imported globals can be exported.
        /// </summary>
        [Fact]
        public void Compile_GlobalImmutableImportExport() {
            var module = new Module();
            module.Imports.Add(new Import.Global {
                Module = "Imported",
                Field = "Global",
                ContentType = WebAssemblyValueType.Int32,
            });
            module.Exports.Add(new Export {
                Name = "Test",
                Kind = ExternalKind.Global,
            });

            var compiled = module.ToInstance<CompilerTestBaseExportedImmutableGlobal<int>>(
                new ImportDictionary {
                    { "Imported", "Global", new GlobalImport(() => ImportedImmutableGlobalReturns3) },
                });

            Assert.NotNull(compiled);
            Assert.NotNull(compiled.Exports);

            var instance = compiled.Exports;

            Assert.Equal(ImportedImmutableGlobalReturns3, instance.Test);
        }

        /// <summary>
        /// Used by <see cref="Compile_GlobalMutableImportExport"/>.
        /// </summary>
        public static int MutableGlobal { get; set; }

        /// <summary>
        /// Verifies that imported globals can be exported.
        /// </summary>
        [Fact]
        public void Compile_GlobalMutableImportExport() {
            var module = new Module();
            module.Imports.Add(new Import.Global {
                Module = "Imported",
                Field = "Global",
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = true,
            });
            module.Exports.Add(new Export {
                Name = "Test",
                Kind = ExternalKind.Global,
            });

            var compiled = module.ToInstance<CompilerTestBaseExportedMutableGlobal<int>>(
                new ImportDictionary {
                    { "Imported", "Global", new GlobalImport(() => MutableGlobal, value => MutableGlobal = value) },
                });

            Assert.NotNull(compiled);
            Assert.NotNull(compiled.Exports);

            var instance = compiled.Exports;

            Assert.Equal(0, instance.Test);

            const int passedThroughImport = 5;
            MutableGlobal = passedThroughImport;
            Assert.Equal(passedThroughImport, instance.Test);

            const int passedThroughExport = 7;
            instance.Test = passedThroughExport;
            Assert.Equal(passedThroughExport, MutableGlobal);
        }
    }

}
