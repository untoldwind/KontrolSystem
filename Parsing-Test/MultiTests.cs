using NUnit.Framework;
using KontrolSystem.Parsing;
using System.Collections.Generic;

namespace KontrolSystem.Parsing.Test {
    using static Parsing.Parsers;

    [TestFixture]
    public class MultiTests {
        [Test]
        public void TextMany0() {
            var parser = Many0(Char('B'));
            var result = parser.TryParse("");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(new List<char>(), result.Value);

            result = parser.TryParse("abcde");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("abcde", result.Remaining.ToString());
            Assert.AreEqual(new List<char>(), result.Value);

            result = parser.TryParse("BBBde");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("de", result.Remaining.ToString());
            Assert.AreEqual(new List<char>(new char[] { 'B', 'B', 'B' }), result.Value);
        }

        [Test]
        public void TestMany1() {
            var parser = Many1(Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abcde");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("Bbcde");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("bcde", result.Remaining.ToString());
            Assert.AreEqual(new List<char>(new char[] { 'B' }), result.Value);

            result = parser.TryParse("BBBde");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("de", result.Remaining.ToString());
            Assert.AreEqual(new List<char>(new char[] { 'B', 'B', 'B' }), result.Value);
        }

        [Test]
        public void TestDelimited0() {
            var parser = Delimited0(OneOf("abAB"), Char(',').Between(WhiteSpaces0, WhiteSpaces0));
            var result = parser.TryParse("");

            Assert.True(result.WasSuccessful);
            Assert.IsEmpty(result.Value);

            result = parser.TryParse("a, e");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(", e", result.Remaining.ToString());
            Assert.AreEqual(new List<char> { 'a' }, result.Value);

            result = parser.TryParse("a,");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(",", result.Remaining.ToString());
            Assert.AreEqual(new List<char> { 'a' }, result.Value);

            result = parser.TryParse("a , b,a, A ,B");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(new List<char> { 'a', 'b', 'a', 'A', 'B' }, result.Value);
        }
    }
}
