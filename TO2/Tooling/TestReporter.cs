using System.Collections.Generic;
using System.IO;

namespace KontrolSystem.TO2.Tooling {
    public interface ITestReporter {
        void BeginModule(string moduleName);

        void EndModule(string moduleName);

        void Report(TestResult testResult);
    }

    public class ConsoleTestReporter : ITestReporter {
        private readonly TextWriter output;
        private readonly List<TestResult> failures = new List<TestResult>();
        private readonly List<TestResult> errors = new List<TestResult>();

        public ConsoleTestReporter(TextWriter _output) => output = _output;

        public void BeginModule(string moduleName) {
            output.WriteLine($"Module {moduleName}");
            output.WriteLine();
        }

        public void EndModule(string moduleName) {
            output.WriteLine();
        }

        public void Report(TestResult testResult) {
            foreach (string message in testResult.messages)
                output.WriteLine(message);
            switch (testResult.state) {
            case TestResultState.Success:
                output.WriteLine($"    {(testResult.testName + "  ").PadRight(65, '.')}  Success ({testResult.successfulAssertions} assertions)");
                break;
            case TestResultState.Failure:
                failures.Add(testResult);
                output.WriteLine($"    {(testResult.testName + "  ").PadRight(65, '.')}  Failure (after {testResult.successfulAssertions} assertions)");
                break;
            case TestResultState.Error:
                errors.Add(testResult);
                output.WriteLine($"    {(testResult.testName + "  ").PadRight(65, '.')}  Error (after {testResult.successfulAssertions} assertions)");
                break;
            }
        }

        public bool WasSuccessful => failures.Count == 0 && errors.Count == 0;

        public List<TestResult> Failures => failures;

        public List<TestResult> Errors => errors;
    }
}
