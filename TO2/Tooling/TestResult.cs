using System;
using System.Collections.Generic;

namespace KontrolSystem.TO2.Tooling {
    public enum TestResultState {
        Success,
        Failure,
        Error,
    }

    public struct TestResult {
        public readonly TestResultState state;
        public readonly string testName;
        public readonly int successfulAssertions;
        public readonly string failure;
        public readonly Exception exception;
        public readonly List<string> messages;

        public TestResult(string _testName, int _successfulAssertions, List<string> _messages) {
            state = TestResultState.Success;
            testName = _testName;
            successfulAssertions = _successfulAssertions;
            failure = null;
            exception = null;
            messages = _messages;
        }

        public TestResult(string _testName, int _successfulAssertions, string _failure, List<string> _messages) {
            state = TestResultState.Failure;
            testName = _testName;
            successfulAssertions = _successfulAssertions;
            failure = _failure;
            exception = null;
            messages = _messages;
        }

        public TestResult(string _testName, int _successfulAssertions, Exception _exceptions, List<string> _messages) {
            state = TestResultState.Error;
            testName = _testName;
            successfulAssertions = _successfulAssertions;
            failure = null;
            exception = _exceptions;
            messages = _messages;
        }
    }
}
