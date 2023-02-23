using Xunit;
using WebAssembly.Instructions;
using WebAssembly.Runtime;

namespace WebAssembly.Test.Instructions {
    public class EndTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction.
        /// </summary>
        [Fact]
        public void End_Compiled() {
            AssemblyBuilder.CreateInstance<CompilerTestBaseVoid>("Test", null, new End()).Test();
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction where a value is returned.
        /// </summary>
        [Fact]
        public void End_Compiled_SingleReturn() {
            Assert.Equal<int>(5,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                    new Int32Constant(5),
                    new End()
                    ).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction where multiple values are returned when one is expected.
        /// </summary>
        [Fact]
        public void End_Compiled_IncorrectStack_Expect1Actual2() {
            var exception = Assert.Throws<StackSizeIncorrectException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                    new Int32Constant(5),
                    new Int32Constant(6),
                    new End()
                    ).Test());

            Assert.Equal(1, exception.Expected);
            Assert.Equal(2, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction where no values are returned when one is expected.
        /// </summary>
        [Fact]
        public void End_Compiled_IncorrectStack_Expect1Actual0() {
            var exception = Assert.Throws<StackSizeIncorrectException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                    new End()
                    ).Test());

            Assert.Equal(1, exception.Expected);
            Assert.Equal(0, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction where one value is returned when none are expected.
        /// </summary>
        [Fact]
        public void End_Compiled_IncorrectStack_Expect0Actual1() {
            var exception = Assert.Throws<StackSizeIncorrectException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", null,
                    new Int32Constant(5),
                    new End()
                    ).Test());

            Assert.Equal(0, exception.Expected);
            Assert.Equal(1, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockInt32() {
            Assert.Equal<int>(5,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                    new Block(BlockType.Int32),
                    new Int32Constant(5),
                    new End(),
                    new End()
                    ).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value of the wrong type.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockInt32_WrongType() {
            var exception = Assert.Throws<StackTypeInvalidException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int32,
                    new Block(BlockType.Int32),
                    new Int64Constant(5),
                    new End(),
                    new Int32WrapInt64(),
                    new End()
                    ).Test());

            Assert.Equal(WebAssemblyValueType.Int32, exception.Expected);
            Assert.Equal(WebAssemblyValueType.Int64, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockInt64() {
            Assert.Equal<long>(5,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<long>>("Test", WebAssemblyValueType.Int64,
                    new Block(BlockType.Int64),
                    new Int64Constant(5),
                    new End(),
                    new End()
                    ).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value of the wrong type.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockInt64_WrongType() {
            var exception = Assert.Throws<StackTypeInvalidException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Int64,
                    new Block(BlockType.Int64),
                    new Int32Constant(5),
                    new End(),
                    new Int64ExtendInt32Signed(),
                    new End()
                    ).Test());

            Assert.Equal(WebAssemblyValueType.Int64, exception.Expected);
            Assert.Equal(WebAssemblyValueType.Int32, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockFloat32() {
            Assert.Equal<float>(5,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<float>>("Test", WebAssemblyValueType.Float32,
                    new Block(BlockType.Float32),
                    new Float32Constant(5),
                    new End(),
                    new End()
                    ).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value of the wrong type.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockFloat32_WrongType() {
            var exception = Assert.Throws<StackTypeInvalidException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Float32,
                    new Block(BlockType.Float32),
                    new Float64Constant(5),
                    new End(),
                    new Float32DemoteFloat64(),
                    new End()
                    ).Test());

            Assert.Equal(WebAssemblyValueType.Float32, exception.Expected);
            Assert.Equal(WebAssemblyValueType.Float64, exception.Actual);
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockFloat64() {
            Assert.Equal<double>(5,
                AssemblyBuilder.CreateInstance<CompilerTestBase0<double>>("Test", WebAssemblyValueType.Float64,
                    new Block(BlockType.Float64),
                    new Float64Constant(5),
                    new End(),
                    new End()
                    ).Test());
        }

        /// <summary>
        /// Tests compilation and execution of the <see cref="End"/> instruction when used with <see cref="Block"/> to retain a value of the wrong type.
        /// </summary>
        [Fact]
        public void End_Compiled_BlockFloat64_WrongType() {
            var exception = Assert.Throws<StackTypeInvalidException>(() =>
                AssemblyBuilder.CreateInstance<CompilerTestBase0<int>>("Test", WebAssemblyValueType.Float64,
                    new Block(BlockType.Float64),
                    new Float32Constant(5),
                    new End(),
                    new Float64PromoteFloat32(),
                    new End()
                    ).Test());

            Assert.Equal(WebAssemblyValueType.Float64, exception.Expected);
            Assert.Equal(WebAssemblyValueType.Float32, exception.Actual);
        }
    }

}
