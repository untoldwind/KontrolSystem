using Xunit;
using System;
using System.Text;
using System.Reflection;
using WebAssembly.Runtime;

namespace WebAssembly.Test {
    public abstract class SampleIssue7Interface {
        public abstract int main();
    }
    /// <summary>
    /// Verifies proper functionality when handling small externally-generate WASM sources.
    /// </summary>
    public class SampleTests {
        /// <summary>
        /// The data acquired from calls to <see cref="Issue7Receive(int)"/>
        /// </summary>
        private static readonly StringBuilder issue7Received = new StringBuilder();

        /// <summary>
        /// Used with <see cref="Execute_Sample_Issue7"/> to verify a call out from a WebAssembly file.
        /// </summary>
        /// <param name="value">The input.</param>
        public static void Issue7Receive(int value) {
            Assert.NotNull(issue7Received);

            issue7Received.Append((char)value);
        }

        /// <summary>
        /// Verifies proper parsing of the sample provided via https://github.com/RyanLamansky/dotnet-webassembly/issues/7 .
        /// This sample was produced via a very simple program built with https://webassembly.studio/ .
        /// </summary>
        [Fact]
        public void Parse_Sample_Issue7() {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebAssembly.Test.Samples.Issue7.wasm");
            Assert.NotNull(stream);
            var module = Module.ReadFromBinary(stream!);

            Assert.Equal(2, module.Codes.Count);
            Assert.Equal(9, module.CustomSections.Count);
            Assert.Equal(0, module.Data.Count);
            Assert.Equal(0, module.Elements.Count);
            Assert.Equal(4, module.Exports.Count);
            Assert.Equal(2, module.Functions.Count);
            Assert.Equal(1, module.Imports.Count);
            Assert.Equal(1, module.Memories.Count);
            Assert.Null(module.Start);
            Assert.Equal(1, module.Tables.Count);
            Assert.Equal(3, module.Types.Count);
        }

        /// <summary>
        /// Verifies proper functionality of the sample provided via https://github.com/RyanLamansky/dotnet-webassembly/issues/7 .
        /// This sample was produced via a very simple program built with https://webassembly.studio/ .
        /// </summary>
        [Fact]
        public void Execute_Sample_Issue7() {
            Assert.Equal(0, issue7Received.Length);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebAssembly.Test.Samples.Issue7.wasm")) {
                var imports = new ImportDictionary
                    {
                    { "env", "sayc", new FunctionImport(new Action<int>(Issue7Receive)) },
                };
                Assert.NotNull(stream);
                var compiled = Compile.FromBinary<SampleIssue7Interface>(stream!)(imports);
                Assert.Equal<int>(0, compiled.Exports.main());
            }

            Assert.Equal("Hello World (from WASM)\n", issue7Received.ToString());
        }
    }

}
