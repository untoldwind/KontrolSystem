using System.IO;
using NUnit.Framework;
using KontrolSystem.TO2.Tooling;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class YieldTimeoutTests {
        static string to2BaseDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "to2SelfTest");

        [Test]
        public void SyncInfiniteLoop() {
            var function = SetupModule().FindFunction("sync_infinite_loop");
            Assert.NotNull(function);

            var result = function.Invoke(TestRunner.DefaultTestContextFactory(), 10L);
            Assert.AreEqual(55L, result);

            Assert.Throws(typeof(YieldTimeoutException), () => {
                function.Invoke(TestRunner.DefaultTestContextFactory(), -10L);
            }, "Expected yield timeout");
        }

        [Test]
        public void AsyncInfiniteLoop() {
            var function = SetupModule().FindFunction("async_infinite_loop");
            Assert.NotNull(function);

            var context = TestRunner.DefaultTestContextFactory();
            var future = function.Invoke(context, 10L) as Future<long>;
            int i;
            long result = 0;

            for (i = 0; i < 20; i++) {
                context.ResetTimeout();
                var pollResult = future.PollValue();
                if (pollResult.IsReady) {
                    result = pollResult.value;
                    break;
                }
            }
            Assert.AreEqual(55L, result);
            Assert.AreEqual(9, i);

            Assert.Throws(typeof(YieldTimeoutException), () => {
                var nextFuture = function.Invoke(context, -10L) as Future<long>;

                for (i = 0; i < 20; i++) {
                    context.ResetTimeout();
                    if (nextFuture.PollValue().IsReady) break;
                }
            }, "Expected yield timeout");
        }

        private IKontrolModule SetupModule() {
            try {
                var registry = KontrolRegistry.CreateCore();

                registry.RegisterModule(BindingGenerator.BindModule(typeof(TestModule)));

                registry.AddFile(to2BaseDir, "Test-Timeout.to2");

                var kontrolModule = registry.modules["test_timeout"];
                Assert.NotNull(kontrolModule);

                return kontrolModule;
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    TestContext.Error.WriteLine(error);
                }
                throw e;
            }
        }
    }
}
