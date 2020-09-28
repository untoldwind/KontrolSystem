using NUnit.Framework;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2ParserLiteralTests {
        [Test]
        public void TestLiteralString() {
            var result = TO2ParserLiterals.LiteralString.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralString.TryParse("\"\"");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("", result.Value.value);

            result = TO2ParserLiterals.LiteralString.TryParse("\"abcd\"");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("abcd", result.Value.value);
        }

        [Test]
        public void TestLiteralInt() {
            var result = TO2ParserLiterals.LiteralInt.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1234L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("-4321");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-4321L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0x1234");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(0x1234L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0b1001");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(9L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0o1234");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(668L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("1_234_567_890");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1234567890L, result.Value.value);
        }

        [Test]
        public void TestLiteralFloat() {
            var result = TO2ParserLiterals.LiteralFloat.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("1234");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse(".1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(.1234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("1.234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1.234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-.1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-.1234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-1.234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-1.234, result.Value.value);
        }

        [Test]
        public void TestLiteralBool() {
            var result = TO2ParserLiterals.LiteralBool.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralBool.TryParse("tru");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralBool.TryParse("true");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.True(result.Value.value);

            result = TO2ParserLiterals.LiteralBool.TryParse("false");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.False(result.Value.value);
        }
    }
}
