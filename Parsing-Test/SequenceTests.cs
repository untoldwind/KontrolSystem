using NUnit.Framework;
using KontrolSystem.Parsing;

namespace KontrolSystem.Parsing.Test {
    using static Parsing.Parsers;

    [TestFixture]
    public class SequenceTests {
        [Test]
        public void TestPreceded() {
            var parser = Preceded(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("c", result.Remaining.ToString());
            Assert.AreEqual('B', result.Value);
        }

        [Test]
        public void TestTerminated() {
            var parser = Terminated(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("c", result.Remaining.ToString());
            Assert.AreEqual('a', result.Value);
        }

        [Test]
        public void TestPair() {
            var parser = Seq(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("c", result.Remaining.ToString());
            Assert.AreEqual(('a', 'B'), result.Value);
        }

        [Test]
        public void TestTriple() {
            var parser = Seq(Char('a'), Char('B'), Char('c'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(('a', 'B', 'c'), result.Value);
        }

        [Test]
        public void TestQuad() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcD");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcd");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(('a', 'B', 'c', 'd'), result.Value);
        }

        [Test]
        public void TestQuint() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcDe");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcdE");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(('a', 'B', 'c', 'd', 'E'), result.Value);
        }

        [Test]
        public void TestHexa() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'), Char('f'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcDef");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcdEf");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Assert.AreEqual(('a', 'B', 'c', 'd', 'E', 'f'), result.Value);
        }
    }
}
