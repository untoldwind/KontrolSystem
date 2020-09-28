using System.Collections.Generic;
using NUnit.Framework;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2ParserExpressionTests {
        static Context emptyContext = new Context(KontrolRegistry.CreateCore());

        static string[] ignorePosition = new string[] { "start", "end", "parentContainer" };

        [Test]
        public void TestExpressionInvalid() {
            var result = TO2ParserExpressions.Expression.TryParse("");

            Assert.False(result.WasSuccessful);
        }

        [Test]
        public void TestExpressionLiterals() {
            var result = TO2ParserExpressions.Expression.TryParse("1234 ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new LiteralInt(1234), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("1234.56 ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new LiteralFloat(1234.56), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("\"ab c de\" ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new LiteralString("ab c de"), result.Value, ignorePosition);
        }

        [Test]
        public void TestAddSub() {
            var result = TO2ParserExpressions.Expression.TryParse("1234 + 4321");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new LiteralInt(1234), Operator.Add, new LiteralInt(4321)), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("1234 + 4321 - 567");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new Binary(new LiteralInt(1234), Operator.Add, new LiteralInt(4321)), Operator.Sub, new LiteralInt(567)), result.Value, ignorePosition);
        }

        [Test]
        public void TestMulDiv() {
            var result = TO2ParserExpressions.Expression.TryParse("1234 * 4321");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new LiteralInt(1234), Operator.Mul, new LiteralInt(4321)), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("1234 + 4321 / 567");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new LiteralInt(1234), Operator.Add, new Binary(new LiteralInt(4321), Operator.Div, new LiteralInt(567))), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("( 1234 + 4321 ) / 567");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new Bracket(new Binary(new LiteralInt(1234), Operator.Add, new LiteralInt(4321))), Operator.Div, new LiteralInt(567)), result.Value, ignorePosition);
        }

        [Test]
        public void TestBlock() {
            var result = TO2ParserExpressions.Expression.TryParse("{ } ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Block(new List<IBlockItem>()), result.Value, ignorePosition);
        }

        [Test]
        public void TestVariable() {
            var result = TO2ParserExpressions.Expression.TryParse("ab ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new VariableGet(new List<string> { "ab" }), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("ab + 12");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new VariableGet(new List<string> { "ab" }), Operator.Add, new LiteralInt(12)), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("ab + 12 * _be13");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new Binary(new VariableGet(new List<string> { "ab" }), Operator.Add, new Binary(new LiteralInt(12), Operator.Mul, new VariableGet(new List<string> { "_be13" }))), result.Value, ignorePosition);
        }

        [Test]
        public void TestMethodInvokation() {
            var result = TO2ParserExpressions.Expression.TryParse("ab . to_int() ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new MethodCall(new VariableGet(new List<string> { "ab" }), "to_int", new List<Expression>()), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("de.amethod(1, 2) ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new MethodCall(new VariableGet(new List<string> { "de" }), "amethod", new List<Expression> { new LiteralInt(1), new LiteralInt(2) }), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("(12 + 56).to_float() ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new MethodCall(new Bracket(new Binary(new LiteralInt(12), Operator.Add, new LiteralInt(56))), "to_float", new List<Expression>()), result.Value, ignorePosition);

            result = TO2ParserExpressions.Expression.TryParse("de.amethod(1, 2).to_something() ");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual(" ", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new MethodCall(new MethodCall(new VariableGet(new List<string> { "de" }), "amethod", new List<Expression> { new LiteralInt(1), new LiteralInt(2) }), "to_something", new List<Expression>()), result.Value, ignorePosition);
        }

        [Test]
        public void TestUnaryNot() {
            var result = TO2ParserExpressions.Expression.TryParse("!ab");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new UnaryPrefix(Operator.Not, new VariableGet(new List<string> { "ab" })), result.Value, ignorePosition);
        }

        [Test]
        public void TestSimpleIf() {
            var result = TO2ParserExpressions.Expression.TryParse("if (!ab) {}");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());

        }
    }
}
