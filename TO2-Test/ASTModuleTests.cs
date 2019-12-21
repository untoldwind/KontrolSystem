using NUnit.Framework;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class ASTModuleTests {
        [Test]
        public void TestBuildModuleName() {
            Assert.AreEqual("test_testcontext", TO2Module.BuildName("Test-TestContext.to2"));
            Assert.AreEqual("sub::mod::demo", TO2Module.BuildName("sub\\mod\\demo.to2"));
            Assert.AreEqual("sub::mod::demo34", TO2Module.BuildName("sub/mod/demo34"));
        }
    }
}
