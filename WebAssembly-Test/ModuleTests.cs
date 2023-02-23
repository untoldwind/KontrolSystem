using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Text;
using WebAssembly.Instructions;

namespace WebAssembly.Test {
    /// <summary>
    /// Tests the <see cref="Module"/> class.
    /// </summary>
    public class ModuleTests {
        /// <summary>
        /// Tests the <see cref="Module.ReadFromBinary(Stream)"/> method.
        /// </summary>
        [Fact]
        public void Module_ReadFromBinaryStream() {
            Assert.Equal("input", Assert.Throws<ArgumentNullException>(() => Module.ReadFromBinary((Stream)null!)).ParamName);

            using var sample = new MemoryStream();
            var utf8 = new UTF8Encoding(false, false);

            using (var writer = new BinaryWriter(sample, utf8, true)) {
                writer.Write(0x6e736100); //Bad magic number.
            }
            sample.Position = 0;
            Assert.True(Assert.Throws<ModuleLoadException>(() => Module.ReadFromBinary(sample)).Message.ToLowerInvariant().Contains("magic"));
            Assert.True(sample.CanSeek, "Stream was closed but should have been left open.");

            sample.Position = 0;
            using (var writer = new BinaryWriter(sample, utf8, true)) {
                writer.Write(0x6d736100);
                //Missing version.
            }
            sample.Position = 0;
            Assert.IsAssignableFrom<EndOfStreamException>(Assert.Throws<ModuleLoadException>(() => Module.ReadFromBinary(sample)).InnerException);

            sample.Position = 0;
            using (var writer = new BinaryWriter(sample, utf8, true)) {
                writer.Write(0x6d736100);
                writer.Write(0x0); //Bad version
            }
            sample.Position = 0;
            Assert.True(Assert.Throws<ModuleLoadException>(() => Module.ReadFromBinary(sample)).Message.ToLowerInvariant().Contains("version"));

            sample.Position = 0;
            using (var writer = new BinaryWriter(sample, utf8, true)) {
                //Shouldn't fail, this is the bare minimum WASM binary file.
                writer.Write(0x6d736100);
                writer.Write(0x1);
            }
            sample.Position = 0;
            Assert.NotNull(Module.ReadFromBinary(sample));

            sample.Position = 0;
            using (var writer = new BinaryWriter(sample, utf8, true)) {
                //Shouldn't fail, this is the bare minimum WASM binary file.
                writer.Write(0x6d736100);
                writer.Write(0xd); //Pre-release version, binary format is otherwise identical to first release.
            }
            sample.Position = 0;
            Assert.NotNull(Module.ReadFromBinary(sample));
        }

        /// <summary>
        /// Ensures that <see cref="CustomSection"/>s are both written and readable.
        /// </summary>
        [Fact]
        public void Module_CustomSectionRoundTrip() {
            var content = BitConverter.DoubleToInt64Bits(Math.PI);
            var toWrite = new Module();
            toWrite.CustomSections.Add(new CustomSection {
                Content = BitConverter.GetBytes(content),
                Name = "Test",
            });

            Module toRead;
            using (var memory = new MemoryStream()) {
                toWrite.WriteToBinary(memory);
                memory.Position = 0;

                toRead = Module.ReadFromBinary(memory);
            }

            Assert.NotNull(toRead);
            Assert.NotSame(toWrite, toRead);
            Assert.NotNull(toRead.CustomSections);
            Assert.Equal(1, toRead.CustomSections.Count);

            var custom = toRead.CustomSections[0];
            Assert.NotNull(custom);
            Assert.Equal("Test", custom.Name);
            Assert.NotNull(custom.Content);
            Assert.Equal(8, custom.Content.Count);
            Assert.Equal(content, BitConverter.ToInt64(custom.Content.ToArray(), 0));
        }

        /// <summary>
        /// Verifies that the import types (<see cref="ExternalKind.Function"/>, <see cref="ExternalKind.Table"/>, <see cref="ExternalKind.Memory"/>, and <see cref="ExternalKind.Global"/> are both written and readable.
        /// </summary>
        [Fact]
        public void Module_ImportRoundTrip() {
            var source = new Module {
                Imports = new Import[]
                {
                    new Import.Function
                    {
                        Module = "A",
                        Field = "1",
                        TypeIndex = 2,
                    },
                    new Import.Table
                    {
                        Module = "B",
                        Field = "2",
                        Definition = new Table(1, 2),
                    },
                    new Import.Memory
                    {
                        Module = "C",
                        Field = "3",
                        Type = new Memory(4, 5),
                    },
                    new Import.Global
                    {
                        Module = "D",
                        Field = "4",
                    },
                }
            };

            Module destination;
            using (var stream = new MemoryStream()) {
                source.WriteToBinary(stream);
                stream.Position = 0;

                destination = Module.ReadFromBinary(stream);
            }

            Assert.NotNull(destination);
            Assert.NotSame(source, destination);
            Assert.NotNull(destination.Imports);
            var imports = destination.Imports;
            Assert.NotSame(source.Imports, imports);
            Assert.Equal(4, imports.Count);

            Assert.IsAssignableFrom<Import.Function>(imports[0]);
            {
                var function = (Import.Function)imports[0];
                Assert.Equal("A", function.Module);
                Assert.Equal("1", function.Field);
                Assert.Equal(2u, function.TypeIndex);
            }

            Assert.IsAssignableFrom<Import.Table>(imports[1]);
            {
                var table = (Import.Table)imports[1];
                Assert.Equal("B", table.Module);
                Assert.Equal("2", table.Field);
                Assert.NotNull(table.Definition);
                Assert.Equal(ElementType.FunctionReference, table.Definition!.ElementType);
                Assert.NotNull(table.Definition.ResizableLimits);
                Assert.Equal(1u, table.Definition.ResizableLimits.Minimum);
                Assert.Equal(2u, table.Definition.ResizableLimits.Maximum.GetValueOrDefault());
            }

            Assert.IsAssignableFrom<Import.Memory>(imports[2]);
            {
                var memory = (Import.Memory)imports[2];
                Assert.Equal("C", memory.Module);
                Assert.Equal("3", memory.Field);
                Assert.NotNull(memory.Type);
                Assert.NotNull(memory.Type!.ResizableLimits);
                Assert.Equal(4u, memory.Type.ResizableLimits.Minimum);
                Assert.Equal(5u, memory.Type.ResizableLimits.Maximum.GetValueOrDefault());
            }

            Assert.IsAssignableFrom<Import.Global>(imports[3]);
            {
                var global = (Import.Global)imports[3];
                Assert.Equal("D", global.Module);
                Assert.Equal("4", global.Field);
            }
        }

        /// <summary>
        /// Verifies that <see cref="Module.Types"/> contents are round-tripped correctly.
        /// </summary>
        [Fact]
        public void Module_TypeRoundTrip() {
            var source = new Module {
                Types = new WebAssemblyType[]
                {
                    new WebAssemblyType
                    {
                        Parameters = new[] { WebAssemblyValueType.Int32, WebAssemblyValueType.Float32 },
                        Returns = new[] { WebAssemblyValueType.Int64 }
                    }
                }
            }; ;

            Module destination;
            using (var stream = new MemoryStream()) {
                source.WriteToBinary(stream);
                stream.Position = 0;

                destination = Module.ReadFromBinary(stream);
            }

            Assert.NotNull(destination.Types);
            Assert.NotSame(source.Types, destination.Types);
            Assert.Equal(1, destination.Types.Count);
            Assert.True(source.Types[0].Equals(destination.Types[0]));
        }

        /// <summary>
        /// Verifies that <see cref="Module.Codes"/> contents are round-tripped correctly.
        /// </summary>
        [Fact]
        public void Module_FunctionBodyRoundTrip() {
            var source = new Module {
                Codes = new[]
                {
                    new FunctionBody
                    {
                        Locals = new[]
                        {
                            new Local
                            {
                                Count  = 2,
                                Type = WebAssemblyValueType.Float64
                            }
                        },
                        Code = new Instruction[]
                        {
                            new End()
                        }
                    }
                }
            };

            Module destination;
            using (var stream = new MemoryStream()) {
                source.WriteToBinary(stream);
                stream.Position = 0;

                destination = Module.ReadFromBinary(stream);
            }

            Assert.NotNull(destination.Codes);
            Assert.NotSame(source.Codes, destination.Codes);
            TestUtility.AreEqual(source.Codes[0], destination.Codes[0]);
        }

        /// <summary>
        /// Verifies that the module writing process prevents an attempt to write an unreadable assembly that contains an instruction sequence that doesn't end with <see cref="OpCode.End"/>.
        /// </summary>
        [Fact]
        public void Module_InstructionSequenceMissingEndValidation() {
            Assert.Throws<InvalidOperationException>(() => new Module { Globals = new[] { new Global() } }.WriteToBinaryNoOutput());
            Assert.Throws<InvalidOperationException>(() => new Module { Elements = new[] { new Element() } }.WriteToBinaryNoOutput());
            Assert.Throws<InvalidOperationException>(() => new Module { Codes = new[] { new FunctionBody() } }.WriteToBinaryNoOutput());
            Assert.Throws<InvalidOperationException>(() => new Module { Data = new[] { new Data() } }.WriteToBinaryNoOutput());
        }
    }

}
