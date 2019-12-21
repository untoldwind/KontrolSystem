using System;
using NUnit.Framework;
using KontrolSystem.Parsing;

namespace KontrolSystem.Parsing.Test {
    using static Parsing.Parsers;

    [TestFixture]
    public class CompleteTests {
        [Test]
        public void TestChar() {
            var parser = Char('A');
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABC");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("BC", result.Remaining.ToString());
            Assert.AreEqual('A', result.Value);
        }

        [Test]
        public void TestTag() {
            var parser = Tag("abc");
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ab");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abc");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual("abc", result.Value);

            result = parser.TryParse("abcde");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("de", result.Remaining.ToString());
            Assert.AreEqual("abc", result.Value);
        }

        [Test]
        public void TestWhitespaces() {
            var parser = WhiteSpaces0;
            var result = parser.TryParse(" \t\r\n");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(" \t\r\n", result.Value);
        }
    }
}
