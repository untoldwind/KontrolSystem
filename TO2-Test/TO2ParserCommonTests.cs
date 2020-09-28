using NUnit.Framework;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2ParserCommonTests {
        [Test]
        public void TestIdentifier() {
            var result = TO2ParserCommon.Identifier.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserCommon.Identifier.TryParse("12ab");

            Assert.False(result.WasSuccessful);

            result = TO2ParserCommon.Identifier.TryParse("ab12_");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("ab12_", result.Value);

            result = TO2ParserCommon.Identifier.TryParse("_12ab");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("_12ab", result.Value);
        }
    }
}
