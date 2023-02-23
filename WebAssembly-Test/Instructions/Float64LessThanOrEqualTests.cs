﻿using Xunit;
using System;
using WebAssembly.Instructions;


namespace WebAssembly.Test.Instructions {

    /// <summary>
    /// Tests the <see cref="Float64LessThanOrEqual"/> instruction.
    /// </summary>
    public class Float64LessThanOrEqualTests {
        /// <summary>
        /// Tests compilation and execution of the <see cref="Float64LessThanOrEqual"/> instruction.
        /// </summary>
        [Fact]
        public void Float64LessThanOrEqual_Compiled() {
            var exports = ComparisonTestBase<double>.CreateInstance(new LocalGet(0), new LocalGet(1),
                new Float64LessThanOrEqual(), new End());

            var values = new[] {
                0.0, 1.0, -1.0, -Math.PI, Math.PI, double.NaN, double.NegativeInfinity, double.PositiveInfinity,
                double.Epsilon, -double.Epsilon,
            };

            foreach (var comparand in values) {
                foreach (var value in values) Assert.Equal(comparand <= value, exports.Test(comparand, value) != 0);

                foreach (var value in values) Assert.Equal(value <= comparand, exports.Test(value, comparand) != 0);
            }
        }
    }
}
