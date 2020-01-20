using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    public class TestRunnerContext : IContext, ITO2Logger {
        private readonly ConcurrentQueue<String> messages = new ConcurrentQueue<string>();
        private long loopCounter = 0;
        private Stopwatch timeStopwatch = Stopwatch.StartNew();
        private long timeoutMillis = 100;

        protected int assertionsCount = 0;

        protected int yieldCount = 0;

        public ITO2Logger Logger => this;

        public bool IsBackground => false;

        public int AssertionsCount => assertionsCount;

        public int YieldCount => yieldCount;

        public IEnumerable<string> Messages => messages;

        public void IncrAssertions() => assertionsCount++;

        public void IncrYield() => yieldCount++;

        public void Debug(string message) => messages.Enqueue("DEBUG: " + message);

        public void Info(string message) => messages.Enqueue("INFO: " + message);

        public void Warning(string message) => messages.Enqueue("WARNING: " + message);

        public void Error(string message) => messages.Enqueue("ERROR: " + message);

        public void LogException(Exception exception) {
            messages.Enqueue(exception.Message);
            messages.Enqueue(exception.StackTrace);
        }

        public void CheckTimeout() {
            loopCounter++;
            if (loopCounter > 10000) {
                loopCounter = 0;
                long elapsed = timeStopwatch.ElapsedMilliseconds;
                if (elapsed >= timeoutMillis)
                    throw new YieldTimeoutException(elapsed);
            }
        }

        public void ResetTimeout() {
            loopCounter = 0;
            timeStopwatch.Reset();
            timeStopwatch.Start();
        }

        public IContext CloneBackground(CancellationToken token) => new BackgroundTestContext(this, token);
    }

    public class BackgroundTestContext : IContext {
        private readonly ITO2Logger logger;
        private readonly CancellationToken token;
        private object nextYield;

        public BackgroundTestContext(ITO2Logger _logger, CancellationToken _token) {
            logger = _logger;
            token = _token;
        }

        public ITO2Logger Logger => logger;

        public bool IsBackground => true;

        public void CheckTimeout() { }

        public void ResetTimeout() { }

        public IContext CloneBackground(CancellationToken token) => new BackgroundTestContext(logger, token);
    }

    [KSModule("core::testing",
        Description = "Provides basic assertions for testing. All functions provided by this module should be only used by `test` function."
    )]
    public class CoreTesting {
        public static TestRunnerContext TestContext => ContextHolder.CurrentContext.Value as TestRunnerContext;

        [KSFunction(
            Description = "Assert that `actual` is true (Test only)"
        )]
        public static void assert_true(bool actual) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_true: called without context");
            if (!actual) throw new AssertException("assert_true failed");
        }

        [KSFunction(
            Description = "Assert that `actual` is false (Test only)"
        )]
        public static void assert_false(bool actual) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_false: called without context");
            if (actual) throw new AssertException("assert_false failed");
        }

        [KSFunction(
            Description = "Assert that `actual` string is equal to `expected` (Test only)"
        )]
        public static void assert_string(string expected, string actual) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_string: called without context");
            if (expected != actual) throw new AssertException($"assert_string: {expected} != {actual}");
        }

        [KSFunction(
            Description = "Assert that `actual` integer is equal to `expected` (Test only)"
        )]
        public static void assert_int(long expected, long actual) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_int: called without context");
            if (expected != actual) throw new AssertException($"assert_int: {expected} != {actual}");
        }

        [KSFunction(
            Description = "Assert that `actual` float is almost equal to `expected` with an absolute tolerance of `delta` (Test only)"
        )]
        public static void assert_float(double expected, double actual, double delta = 1e-10) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_float: called without context");
            if (Math.Abs(expected - actual) > delta) throw new AssertException($"assert_float: {expected} != {actual} +/- {delta}");
        }

        [KSFunction]
        public static void assert_some_int(long expected, Option<long> actual) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_some_int: called without context");
            if (!actual.defined) throw new AssertException($"assert_some_int: Some({expected}) != None");
            if (expected != actual.value) throw new AssertException($"assert_some_int: Some({expected}) != Some({actual})");
        }

        [KSFunction(
            Description = "Fail the test case with a `message` (Test only)."
        )]
        public static void failTest(string message) {
            throw new AssertException($"fail: {message}");
        }

        [KSFunction(
            Description = "Assert that test case has yielded `expected` number of times already (Async test only)"
        )]
        public static void assert_yield(long expected) {
            if (TestContext != null) TestContext.IncrAssertions(); else throw new AssertException("assert_some_int: called without context");
            if (TestContext.YieldCount != expected) throw new AssertException($"assert_yield: Expected test to have yield {expected} times, actually there had been {TestContext.YieldCount} yields");
        }

        [KSFunction(
            Description = "Yield the test case (Async test only)"
        )]
        public static Future<object> Yield() => new Future.Success<object>(null);

        [KSFunction(
            Description = "Suspend execution for `millis`"
        )]
        public static void TestSleep(long millis) => Thread.Sleep((int)millis);
    }

    public class AssertException : System.Exception {
        public AssertException(string message) : base(message) { }
    }
}
