﻿using Xunit;
using WebAssembly.Instructions;
using System;

namespace WebAssembly.Test.Instructions {
    public class Float32AbsoluteTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float32Absolute"/> instruction.
        /// </summary>
        [Fact]
        public void Float32Absolute_Compiled() {
            var exports = CompilerTestBase<float>.CreateInstance(
                new LocalGet(0),
                new Float32Absolute(),
                new End());

            foreach (var value in new[] { 1f, -1f, -(float)Math.PI, (float)Math.PI })
                Assert.Equal(Math.Abs(value), exports.Test(value));
        }
    }

}
