using System.IO;
using Xunit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Tooling;
using KontrolSystem.KSP.Runtime.Testing;
using Xunit.Abstractions;

namespace KontrolSystem.KSP.Runtime.Test {
    public class TO2StdTestSuite {
        private readonly ITestOutputHelper output;

        public TO2StdTestSuite(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void RunSuite() {
            ConsoleTestReporter reporter = new ConsoleTestReporter(output.WriteLine);

            try {
                var registry = KontrolSystemKSPRegistry.CreateKSP();

                var context = registry.AddDirectory(Path.Combine("..", "..", "GameData", "KontrolSystem", "to2"));

                context.Save("demo.dll");

                TestRunner.RunTests(registry, reporter, () => new KSPTestRunnerContext());
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }
                throw new Xunit.Sdk.XunitException(e.Message);
            }

            if (!reporter.WasSuccessful) {
                if (reporter.Failures.Count > 0) {
                    output.WriteLine("");
                    output.WriteLine("Failures:");
                    output.WriteLine("");

                    foreach (TestResult failure in reporter.Failures) {
                        output.WriteLine($"    {failure.testName}:");
                        output.WriteLine($"         {failure.failure}");
                    }
                }
                if (reporter.Errors.Count > 0) {
                    output.WriteLine("");
                    output.WriteLine("Errors:");
                    output.WriteLine("");

                    foreach (TestResult error in reporter.Errors) {
                        output.WriteLine($"    {error.testName}:");
                        output.WriteLine(error.exception.ToString());
                    }
                }

                throw new Xunit.Sdk.XunitException("KSSTestSuite was not successful");
            }
        }

    }

}
