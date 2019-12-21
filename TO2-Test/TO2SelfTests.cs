using NUnit.Framework;
using System.IO;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2SelfTests {
        static string to2BaseDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "to2SelfTest");

        static string demo_target(long a) {
            return $"Called with {a}";
        }

        [Test]
        public void TestTestContext() {
            var registry = KontrolRegistry.CreateCore();

            Assert.Contains("core::testing", registry.modules.Keys);

            try {
                Context context = registry.AddFile(to2BaseDir, "Test-TestContext.to2");

                Assert.Contains("test_testcontext", registry.modules.Keys);

                var kontrolModule = registry.modules["test_testcontext"];

                Assert.NotNull(kontrolModule.FindFunction("test_asserts"));

                var testContext = new TestRunnerContext();
                kontrolModule.FindFunction("test_asserts").Invoke(testContext);

                Assert.AreEqual(9, testContext.AssertionsCount);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    TestContext.Error.WriteLine(error);
                }
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void TestInterop() {
            var registry = KontrolRegistry.CreateCore();

            try {
                registry.AddFile(to2BaseDir, "Test-Interop.to2");

                Assert.Contains("test_interop", registry.modules.Keys);

                var kontrolModule = registry.modules["test_interop"];

                Assert.NotNull(kontrolModule.FindFunction("test_basic_callback"));

                var testContext = new TestRunnerContext();
                kontrolModule.FindFunction("test_basic_callback").Invoke(testContext, new System.Func<long, string>(demo_target));

                Assert.AreEqual(2, testContext.AssertionsCount);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    TestContext.Error.WriteLine(error);
                }
                Assert.Fail(e.Message);
            }
        }
    }
}
