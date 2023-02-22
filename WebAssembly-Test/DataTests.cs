using Xunit;
using WebAssembly.Instructions;

namespace WebAssembly.Test {
    public class DataTests {
        /// <summary>
        /// Ensures that <see cref="Data"/> instances have full mutability when read from a file.
        /// </summary>
        [Fact]
        public void Data_MutabilityFromBinaryFile() {
            var module = new Module {
                Data = new[]
                {
                    new Data
                    {
                        InitializerExpression = new Instruction[]
                        {
                            new Int32Constant(0),
                            new End(),
                        },
                    },
                },
            }.BinaryRoundTrip();

            Assert.NotNull(module.Data);
            Assert.Equal(1, module.Data.Count);

            var data = module.Data[0];
            Assert.NotNull(data);

            var initializerExpression = data.InitializerExpression;
            Assert.NotNull(initializerExpression);
            Assert.Equal(2, initializerExpression.Count);

            //Testing mutability here.
            initializerExpression.Clear();
            initializerExpression.Add(new Int32Constant(0));
        }
    }
}
