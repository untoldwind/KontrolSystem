using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsers;
    using static TO2ParserCommon;
    using static TO2ParserExpressions;

    public static class TO2ParserFunctions {
        private static readonly Parser<string> FnKeyword = Tag("fn").Then(Spacing1);

        private static readonly Parser<string> TestKeyword = Tag("test").Then(Spacing1);

        private static readonly Parser<string> SyncKeyword = Tag("sync").Then(Spacing1);

        private static readonly Parser<(FunctionModifier modifier, bool async)> FunctionPrefix = Alt(
            SyncKeyword.Then(PubKeyword).Then(FnKeyword).To((FunctionModifier.Public, false)),
            SyncKeyword.Then(TestKeyword).Then(FnKeyword).To((FunctionModifier.Test, false)),
            PubKeyword.Then(FnKeyword).To((FunctionModifier.Public, true)),
            PubKeyword.Then(SyncKeyword).Then(FnKeyword).To((FunctionModifier.Public, false)),
            TestKeyword.Then(FnKeyword).To((FunctionModifier.Test, true)),
            TestKeyword.Then(SyncKeyword).Then(FnKeyword).To((FunctionModifier.Test, false)),
            SyncKeyword.Then(FnKeyword).To((FunctionModifier.Private, false)),
            FnKeyword.To((FunctionModifier.Private, true))
        );

        public static readonly Parser<FunctionParameter> FunctionParameter = Seq(
            Identifier, TypeSpec, Opt(WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Expression))
        ).Map((param, start, end) => new FunctionParameter(param.Item1, param.Item2,
            param.Item3.IsDefined ? param.Item3.Value : null, start, end));

        private static readonly Parser<List<FunctionParameter>> FunctionParameters = Char('(').Then(WhiteSpaces0)
            .Then(DelimitedUntil(FunctionParameter, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

        public static readonly Parser<FunctionDeclaration> FunctionDeclaration = Seq(
            DescriptionComment, WhiteSpaces0.Then(FunctionPrefix), Identifier, WhiteSpaces0.Then(FunctionParameters),
            WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(TypeRef),
            WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Expression)
        ).Map((decl, start, end) => new FunctionDeclaration(decl.Item2.modifier, decl.Item2.async, decl.Item3,
            decl.Item1, decl.Item4, decl.Item5, decl.Item6, start, end));
    }
}
