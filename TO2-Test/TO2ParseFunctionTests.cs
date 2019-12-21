using System.Collections.Generic;
using NUnit.Framework;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TO2ParserFunctionTests {
        static string[] ignorePosition = new string[] { "start", "end", "parentContainer" };

        [Test]
        public void TestFunctionParameter() {
            var result = TO2ParserFunctions.functionParameter.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.functionParameter.TryParse("ab");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.functionParameter.TryParse("ab:bool");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionParameter("ab", BuildinType.Bool), result.Value, ignorePosition);

            result = TO2ParserFunctions.functionParameter.TryParse("_12ab : int");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionParameter("_12ab", BuildinType.Int), result.Value, ignorePosition);
        }

        [Test]
        public void TestFunctionDeclaration() {
            var result = TO2ParserFunctions.functionDeclaration.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.functionDeclaration.TryParse("fn demo()->Unit=0");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionDeclaration(FunctionModifier.Private, true, "demo", "", new List<FunctionParameter> { }, BuildinType.Unit, new LiteralInt(0)), result.Value, ignorePosition);

            result = TO2ParserFunctions.functionDeclaration.TryParse("pub  fn _demo23 ( ab : string ) -> int = { 0 }");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionDeclaration(FunctionModifier.Public, true, "_demo23", "", new List<FunctionParameter> { new FunctionParameter("ab", BuildinType.String) }, BuildinType.Int, new Block(new List<IBlockItem> { new LiteralInt(0) })), result.Value, ignorePosition);

            result = TO2ParserFunctions.functionDeclaration.TryParse("pub  fn abc34 ( ab : string, _56 : int ) -> int = { 0 }");

            Assert.True(result.WasSuccessful);
            Assert.AreEqual("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionDeclaration(FunctionModifier.Public, true, "abc34", "", new List<FunctionParameter> { new FunctionParameter("ab", BuildinType.String), new FunctionParameter("_56", BuildinType.Int) }, BuildinType.Int, new Block(new List<IBlockItem> { new LiteralInt(0) })), result.Value, ignorePosition);
        }
    }
}
