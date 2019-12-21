using NUnit.Framework;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2ParserLiteralTests {
        [Test]
        public void TestLiteralString() {
            var result = TO2ParserLiterals.literalString.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalString.TryParse("\"\"");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("", result.Value.value);

            result = TO2ParserLiterals.literalString.TryParse("\"abcd\"");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("abcd", result.Value.value);
        }

        [Test]
        public void TestLiteralInt() {
            var result = TO2ParserLiterals.literalInt.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalInt.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalInt.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalInt.TryParse("1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1234L, result.Value.value);

            result = TO2ParserLiterals.literalInt.TryParse("-4321");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-4321L, result.Value.value);

            result = TO2ParserLiterals.literalInt.TryParse("0x1234");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(0x1234L, result.Value.value);

            result = TO2ParserLiterals.literalInt.TryParse("0b1001");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(9L, result.Value.value);

            result = TO2ParserLiterals.literalInt.TryParse("0o1234");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(668L, result.Value.value);

            result = TO2ParserLiterals.literalInt.TryParse("1_234_567_890");
            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1234567890L, result.Value.value);
        }

        [Test]
        public void TestLiteralFloat() {
            var result = TO2ParserLiterals.literalFloat.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalFloat.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalFloat.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalFloat.TryParse("1234");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalFloat.TryParse(".1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(.1234, result.Value.value);

            result = TO2ParserLiterals.literalFloat.TryParse("1.234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(1.234, result.Value.value);

            result = TO2ParserLiterals.literalFloat.TryParse("-.1234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-.1234, result.Value.value);

            result = TO2ParserLiterals.literalFloat.TryParse("-1.234");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(-1.234, result.Value.value);
        }

        [Test]
        public void TestLiteralBool() {
            var result = TO2ParserLiterals.literalBool.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalBool.TryParse("tru");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.literalBool.TryParse("true");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.True(result.Value.value);

            result = TO2ParserLiterals.literalBool.TryParse("false");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.False(result.Value.value);
        }
    }
}
