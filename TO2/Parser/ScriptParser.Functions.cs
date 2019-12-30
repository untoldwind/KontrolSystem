using System;
using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;
    using static TO2ParserCommon;
    using static TO2ParserExpressions;

    public static class TO2ParserFunctions {
        public static readonly Parser<string> fnKeyword = Tag("fn").Then(Spacing1);

        public static readonly Parser<string> testKeyword = Tag("test").Then(Spacing1);

        public static readonly Parser<string> syncKeyword = Tag("sync").Then(Spacing1);

        public static readonly Parser<(FunctionModifier modifier, bool async)> functionPrefix = Alt(
            syncKeyword.Then(pubKeyword).Then(fnKeyword).Map(_ => (FunctionModifier.Public, false)),
            syncKeyword.Then(testKeyword).Then(fnKeyword).Map(_ => (FunctionModifier.Test, false)),
            pubKeyword.Then(fnKeyword).Map(_ => (FunctionModifier.Public, true)),
            pubKeyword.Then(syncKeyword).Then(fnKeyword).Map(_ => (FunctionModifier.Public, false)),
            testKeyword.Then(fnKeyword).Map(_ => (FunctionModifier.Test, true)),
            testKeyword.Then(syncKeyword).Then(fnKeyword).Map(_ => (FunctionModifier.Test, false)),
            syncKeyword.Then(fnKeyword).Map(_ => (FunctionModifier.Private, false)),
            fnKeyword.Map(_ => (FunctionModifier.Private, true))
        );

        public static readonly Parser<FunctionParameter> functionParameter = Seq(
            identifier, typeSpec, Opt(WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(expression))
        ).Map(param => new FunctionParameter(param.Item1, param.Item2, param.Item3.IsDefined ? param.Item3.Value : null));

        public static readonly Parser<List<FunctionParameter>> functionParameters = Char('(').Then(WhiteSpaces0).Then(DelimitedUntil(functionParameter, commaDelimiter, WhiteSpaces0.Then(Char(')'))));

        public static readonly Parser<FunctionDeclaration> functionDeclaration = Seq(
            descriptionComment, WhiteSpaces0.Then(functionPrefix), identifier, WhiteSpaces0.Then(functionParameters), WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(typeRef), WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(expression)
        ).Map((decl, start, end) => new FunctionDeclaration(decl.Item2.modifier, decl.Item2.async, decl.Item3, decl.Item1, decl.Item4, decl.Item5, decl.Item6, start, end));
    }
}
