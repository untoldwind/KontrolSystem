using System.IO;
using NUnit.Framework;
using KontrolSystem.TO2.Tooling;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2TestSuite {
        [Test]
        public void RunSuite() {
            ConsoleTestReporter reporter = new ConsoleTestReporter(TestContext.Out);

            try {
                var registry = KontrolRegistry.CreateCore();

                registry.RegisterModule(BindingGenerator.BindModule(typeof(TestModule)));

                var context = registry.AddDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, "to2Core"));

                context.Save("demo.dll");

                TestRunner.RunTests(registry, reporter, TestRunner.DefaultTestContextFactory);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    TestContext.Error.WriteLine(error);
                }
                Assert.Fail(e.Message);
            }

            if (!reporter.WasSuccessful) {
                if (reporter.Failures.Count > 0) {
                    TestContext.Error.WriteLine();
                    TestContext.Error.WriteLine("Failures:");
                    TestContext.Error.WriteLine();

                    foreach (TestResult failure in reporter.Failures) {
                        TestContext.Error.WriteLine($"    {failure.testName}:");
                        TestContext.Error.WriteLine($"         {failure.failure}");
                    }
                }
                if (reporter.Errors.Count > 0) {
                    TestContext.Error.WriteLine();
                    TestContext.Error.WriteLine("Errors:");
                    TestContext.Error.WriteLine();

                    foreach (TestResult error in reporter.Errors) {
                        TestContext.Error.WriteLine($"    {error.testName}:");
                        TestContext.Error.WriteLine(error.exception);
                    }
                }

                Assert.Fail("KSSTestSuite was not successful");
            }
        }
    }
}
