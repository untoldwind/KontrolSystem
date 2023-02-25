﻿using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using WebAssembly.Instructions;
using WebAssembly.Runtime;
using WebAssembly.Runtime.Compilation;

namespace WebAssembly.Test {
    public abstract class StartInterface {
        public abstract void Start();
    }

    public abstract class ExportFn {
        public abstract int fn();
    }

    public abstract class ExportMemory {
        public abstract UnmanagedMemory Memory { get; }
    }

    public class CompilerTests {
        /// <summary>
        /// Tests a compilation of an empty assembly.
        /// </summary>
        [Fact]
        public void Compile_Empty() {
            var module = new Module();
            module.ToInstance<object>();
        }

        /// <summary>
        /// Tests a compilation of an empty assembly compiled via <see cref="Module.Compile{TExports}"/>.
        /// </summary>
        [Fact]
        public void Compile_DirectFromModule() {
            new Module().Compile<object>();
        }

        /// <summary>
        /// Tests a compilation of an assembly that contains a single exported function that does nothing.
        /// </summary>
        [Fact]
        public void Compile_MinimalExportedFunction() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
            });
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = nameof(HelloWorldExports.Start)
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                new End(),
                },
            });

            var compiled = module.ToInstance<StartInterface>();

            compiled.Exports.Start();
        }

        /// <summary>
        /// A test class for an exported immutable global.
        /// </summary>
        public abstract class ExportedReadonlyGlobal {
            /// <summary>
            /// The test global.
            /// </summary>
            public abstract int Test { get; }
        }

        /// <summary>
        /// Tests a compilation of an assembly that contains a single immutable exported global.
        /// </summary>
        [Fact]
        public void Compile_MinimalExportedImmutableGlobal() {
            var module = new Module();
            module.Globals.Add(new Global {
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = false,
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(5),
                    new End()
                }
            });
            module.Exports.Add(new Export {
                Name = "Test",
                Kind = ExternalKind.Global,
            });

            var compiled = module.ToInstance<ExportedReadonlyGlobal>();

            Assert.Equal(5, compiled.Exports.Test);

            var native = compiled.Exports.GetType().GetProperty("Test")?.GetCustomAttribute<NativeExportAttribute>();
            Assert.NotNull(native);
            Assert.Equal(ExternalKind.Global, native!.Kind);
            Assert.Equal("Test", native.Name);
        }

        /// <summary>
        /// A test class for an exported mutable global.
        /// </summary>
        public abstract class ExportedMutableGlobal {
            /// <summary>
            /// The test global.
            /// </summary>
            public abstract int Test { get; set; }
        }

        /// <summary>
        /// Tests a compilation of an assembly that contains a single mutable exported global.
        /// </summary>
        [Fact]
        public void Compile_MinimalExportedMutableGlobal() {
            var module = new Module();
            module.Globals.Add(new Global {
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = true,
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(3),
                    new End()
                }
            });
            module.Exports.Add(new Export {
                Name = "Test",
                Kind = ExternalKind.Global,
            });

            var compiled = module.ToInstance<ExportedMutableGlobal>();

            Assert.Equal(3, compiled.Exports.Test);
            Assert.NotEqual(4, compiled.Exports.Test);
            compiled.Exports.Test = 7;
            Assert.Equal(7, compiled.Exports.Test);
            Assert.NotEqual(4, compiled.Exports.Test);
            Assert.NotEqual(3, compiled.Exports.Test);
        }

        /// <summary>
        /// Tests a compilation of an assembly that contains a single internal function that does nothing.
        /// </summary>
        [Fact]
        public void Compile_MinimalInternalFunction() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
            });
            module.Functions.Add(new Function {
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                new End(),
                },
            });

            var compiled = module.ToInstance<dynamic>();
        }

        /// <summary>
        /// Tests a very simple program with a single exported function that returns a number, executed dynamically.
        /// </summary>
        [Fact]
        public void Compile_HelloWorld_Dynamic() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                }
            });
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = nameof(HelloWorldExports.Start)
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                new Int32Constant { Value = 8 },
                new End(),
                },
            });

            var compiled = module.ToInstance<HelloWorldExports>();

            var exports = compiled.Exports;
            Assert.Equal(8, exports.Start());
        }

        /// <summary>
        /// A simple test class.
        /// </summary>
        public abstract class HelloWorldExports {
            /// <summary>
            /// A simple test method.
            /// </summary>
            /// <returns>Should always return "3".</returns>
            public abstract int Start();
        }

        /// <summary>
        /// Tests a very simple program with a single exported function that returns a number, executed statically via <see cref="HelloWorldExports"/>.
        /// </summary>
        [Fact]
        public void Compile_HelloWorld_Static() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                }
            });
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = nameof(HelloWorldExports.Start)
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                new Int32Constant { Value = 3 },
                new End(),
                },
            });

            var compiled = module.ToInstance<HelloWorldExports>();

            var exports = compiled.Exports;
            Assert.Equal(3, exports.Start());
        }

        /// <summary>
        /// Tests a very simple program with a read-only forward-only stream to ensure reliable streaming compilation.
        /// </summary>
        [Fact]
        public void Compile_Streaming() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                }
            });
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = nameof(HelloWorldExports.Start)
            });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                new Int32Constant { Value = 3 },
                new End(),
                },
            });

            var compiled = module.ToInstance<HelloWorldExports>();

            var exports = compiled.Exports;
            Assert.Equal(3, exports.Start());
        }

        /// <summary>
        /// A simple test class.
        /// </summary>
        public abstract class HelloWorldExportsWithConstructor {
            internal int SetByConstructor;

            /// <summary>
            /// Creates a new <see cref="HelloWorldExportsWithConstructor"/> instance.
            /// </summary>
            protected HelloWorldExportsWithConstructor() {
                this.SetByConstructor = 5;
            }
        }

        /// <summary>
        /// Ensures that the parent class's constructor is called by the generated inheritor.
        /// </summary>
        [Fact]
        public void Compile_CallsParentConstructor() {
            var module = new Module();

            var compiled = module.ToInstance<HelloWorldExportsWithConstructor>();

            var exports = compiled.Exports;
            Assert.Equal(5, exports.SetByConstructor);
        }

        /// <summary>
        /// Tests the compiler when linear memory is used.
        /// </summary>
        [Fact]
        public void Compiler_Memory() {
            var module = new Module();
            module.Memories.Add(new Memory(1, 1));
            module.Exports.Add(new Export {
                Name = "Memory",
                Kind = ExternalKind.Memory,
            });

            var compiled = module.ToInstance<ExportMemory>();

            UnmanagedMemory linearMemory;
            using (compiled) {
                Assert.NotNull(compiled);
                var exports = compiled.Exports;
                Assert.NotNull(exports);
                linearMemory = exports.Memory;
                Assert.NotNull(linearMemory);
                Assert.NotEqual(IntPtr.Zero, linearMemory.Start);

                for (var i = 0; i < Memory.PageSize; i += 8)
                    Assert.Equal(0, Marshal.ReadInt64(linearMemory.Start + 8));
            }

            Assert.Equal(IntPtr.Zero, linearMemory.Start);
        }

        /// <summary>
        /// Defends against regression of https://github.com/RyanLamansky/dotnet-webassembly/issues/4 , which revealed a bug in the local parser.
        /// </summary>
        [Fact]
        public void Compiler_GithubIssue4_Locals() {
            var module = new Module();
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                }
            });
            module.Functions.Add(new Function {
            });
            module.Exports.Add(new Export {
                Name = "Test",
            });
            module.Codes.Add(new FunctionBody {
                Locals = new[]
                {
                    new Local
                    {
                        Count = 1,
                        Type = WebAssemblyValueType.Int32,
                    }
                },
                Code = new Instruction[]
                {
                    new LocalGet(0),
                    new End(),
                }
            });

            var compiled = module.ToInstance<CompilerTestBase0<int>>();

            Assert.Equal(0, (int)compiled.Exports.Test());
        }

        /// <summary>
        /// Tests the compiler when a custom section is used.
        /// </summary>
        [Fact]
        public void Compiler_CustomSection() {
            var module = new Module();
            module.CustomSections.Add(new CustomSection {
                Content = BitConverter.GetBytes(Math.PI),
                Name = "Test",
            });

            var compiled = module.ToInstance<dynamic>();

            Assert.NotNull(compiled);

            using (compiled) {
            }
        }

        /// <summary>
        /// Tests the compiler when a start section is used.
        /// </summary>
        [Fact]
        public void Compiler_StartSection() {
            var module = new Module();
            module.Memories.Add(new Memory(1, 1));

            module.Types.Add(new WebAssemblyType {
            });
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                }
            });

            module.Functions.Add(new Function {
            });
            module.Functions.Add(new Function {
                Type = 1,
            });

            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new Int32Constant(1),
                    new Int32Constant(2),
                    new Int32Store(),
                    new End(),
                },
            });

            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new Int32Constant(1),
                    new Int32Load(),
                    new End(),
                },
            });

            module.Start = 0;

            module.Exports.Add(new Export {
                Index = 1,
                Name = "Test",
            });

            var compiled = module.ToInstance<CompilerTestBase0<int>>();

            Assert.NotNull(compiled);

            using (compiled) {
                Assert.Equal<int>(2, compiled.Exports.Test());
            }
        }

        /// <summary>
        /// Tests the compiler when a data section is used.
        /// </summary>
        [Fact]
        public void Compiler_Data() {
            var module = new Module();
            module.Memories.Add(new Memory(1, 1));
            module.Exports.Add(new Export {
                Name = "Memory",
                Kind = ExternalKind.Memory,
            });

            module.Data.Add(new Data {
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(0),
                    new End(),
                },
                RawData = new byte[] { 2 },
            });

            var compiled = module.ToInstance<ExportMemory>();

            UnmanagedMemory linearMemory;
            using (compiled) {
                Assert.NotNull(compiled);
                var exports = compiled.Exports;
                Assert.NotNull(exports);
                linearMemory = exports.Memory;
                Assert.NotNull(linearMemory);
                Assert.NotEqual(IntPtr.Zero, linearMemory.Start);

                Assert.Equal(2, Marshal.ReadInt64(linearMemory.Start));
            }

            Assert.Equal(IntPtr.Zero, linearMemory.Start);
        }

        /// <summary>
        /// Tests the compiler when a data section is used with overlapping segments.
        /// </summary>
        [Fact]
        public void Compiler_DataOverlappedSegments() {
            var module = new Module();
            module.Memories.Add(new Memory(1, 1));
            module.Exports.Add(new Export {
                Name = "Memory",
                Kind = ExternalKind.Memory,
            });

            module.Data.Add(new Data {
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(0),
                    new End(),
                },
                RawData = new byte[] { 1, 2 },
            });
            module.Data.Add(new Data {
                InitializerExpression = new Instruction[]
                 {
                    new Int32Constant(1),
                    new End(),
                 },
                RawData = new byte[] { 3, 4 },
            });

            var compiled = module.ToInstance<ExportMemory>();

            UnmanagedMemory linearMemory;
            using (compiled) {
                Assert.NotNull(compiled);
                var exports = compiled.Exports;
                Assert.NotNull(exports);
                linearMemory = exports.Memory;
                Assert.NotNull(linearMemory);
                Assert.NotEqual(IntPtr.Zero, linearMemory.Start);

                Assert.Equal(0x040301, Marshal.ReadInt64(linearMemory.Start));
            }

            Assert.Equal(IntPtr.Zero, linearMemory.Start);
        }

        /// <summary>
        /// Tests the compiler when a data section is used with insufficient minimum memory.
        /// </summary>
        [Fact]
        public void Compiler_DataMemoryMinimumTooSmall() {
            var module = new Module();
            module.Memories.Add(new Memory(0, 1));
            module.Exports.Add(new Export {
                Name = "Memory",
                Kind = ExternalKind.Memory,
            });

            module.Data.Add(new Data {
                InitializerExpression = new Instruction[]
                {
                    new Int32Constant(0),
                    new End(),
                },
                RawData = new byte[] { 2 },
            });

            var x = Assert.Throws<MemoryAccessOutOfRangeException>(() => module.ToInstance<dynamic>());
            Assert.Equal(1u, x.Offset);
            Assert.Equal(1u, x.Length);
        }

        /// <summary>
        /// Verifies that memory can be imported and exported.
        /// </summary>
        [Fact]
        public void Compiler_MemoryImportExport() {
            var module = new Module();
            module.Imports.Add(new Import.Memory {
                Field = "Memory",
                Module = "Memory",
                Type = new Memory(0, 1)
            });
            module.Exports.Add(new Export {
                Name = "Memory",
                Kind = ExternalKind.Memory,
            });

            var memory = new UnmanagedMemory(0, 1);

            var roundMemory = module.ToInstance<ExportMemory>(new ImportDictionary {
                { "Memory", "Memory", new MemoryImport(() => memory) },
            }).Exports.Memory as UnmanagedMemory;
            Assert.NotNull(roundMemory);
            Assert.Same(memory, roundMemory);
        }

        /// <summary>
        /// Verifies that during compilation we read the number of globals
        /// exactly as defined in the global section, even when there is a
        /// global import.
        /// See https://github.com/RyanLamansky/dotnet-webassembly/issues/35
        /// </summary>
        [Fact]
        public void Compiler_ReadGlobalSectionWhenGlobalImport() {
            // Set up a module with a global import and a global
            var module = new Module();
            module.Imports.Add(new WebAssembly.Import.Global("mod", "field"));
            module.Globals.Add(new Global {
                ContentType = WebAssemblyValueType.Int32,
                IsMutable = false,
                InitializerExpression = new Instruction[] { new Int32Constant(1), new End() }.ToList()
            });
            module.Types.Add(new WebAssemblyType {
                Returns = new[]
                {
                    WebAssemblyValueType.Int32,
                },
            });
            module.Functions.Add(new Function { Type = 0 });
            module.Codes.Add(new FunctionBody {
                Code = new Instruction[]
                {
                    new GlobalGet(0),
                    new GlobalGet(1),
                    new Int32Add(),
                    new End(),
                },
            });
            module.Exports.Add(new Export {
                Kind = ExternalKind.Function,
                Index = 0,
                Name = "fn",
            });

            using var memoryStream = new MemoryStream();
            module.WriteToBinary(memoryStream);
            memoryStream.Seek(0L, SeekOrigin.Begin);

            var compilationResult = Compile.FromBinary<ExportFn>(memoryStream);
            var result = compilationResult.Invoke(new Dictionary<string, IDictionary<string, RuntimeImport>>
                {
                { "mod", new Dictionary<string, RuntimeImport> { { "field", new GlobalImport(() => 2) } } },
            }).Exports.fn();
            Assert.Equal(3, result);
        }
    }

}