using System;
using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;
    using static TO2ParserCommon;
    using static TO2ParserExpressions;
    using static TO2ParserFunctions;

    public static class TO2ParserModule {
        public static Parser<string> useKeyword = Tag("use").Then(Spacing1);

        public static Parser<String> typeKeyword = Tag("type").Then(Spacing1);

        public static Parser<string> formKeyword = Spacing1.Then(Tag("from")).Then(Spacing1);

        public static Parser<string> asKeyword = Spacing1.Then(Tag("as")).Then(Spacing1);

        public static Parser<List<string>> useNames = Alt(
            Char('*').Map(_ => (List<string>)null),
            Delimited1(identifier, Char(',').Between(WhiteSpaces0, WhiteSpaces0)).Between(Char('{').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('}')))
        );

        public static Parser<UseDeclaration> useNamesDeclaration = Seq(
            useKeyword.Then(useNames), formKeyword.Then(identifierPath)
        ).Map((decl, start, end) => new UseDeclaration(decl.Item1, decl.Item2, start, end));

        public static Parser<UseDeclaration> useAliasDeclaration = Seq(
            useKeyword.Then(identifierPath), asKeyword.Then(identifier)
        ).Map((decl, start, end) => new UseDeclaration(decl.Item1, decl.Item2, start, end));

        public static Parser<TypeAlias> typeAlias = Seq(
             descriptionComment, WhiteSpaces0.Then(Opt(pubKeyword)), typeKeyword.Then(identifier), WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(typeRef)
        ).Map((items, start, end) => new TypeAlias(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, start, end));

        public static Parser<ConstDeclaration> constDeclaration = Seq(
            descriptionComment, Opt(pubKeyword), Tag("const").Then(WhiteSpaces1).Then(identifier), typeSpec, WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(expression)
        ).Map((items, start, end) => new ConstDeclaration(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, items.Item5, start, end));

        public static Parser<IModuleItem> moduleItem = Alt<IModuleItem>(
            useNamesDeclaration,
            useAliasDeclaration,
            functionDeclaration,
            typeAlias,
            constDeclaration,
            lineComment
        );

        public static Parser<List<IModuleItem>> moduleItems = WhiteSpaces0.Then(DelimitedUntil(moduleItem, WhiteSpaces1, EOF));

        public static Parser<TO2Module> module(string moduleName) => Seq(WhiteSpaces0.Then(descriptionComment), moduleItems).Map(items => new TO2Module(moduleName, items.Item1, items.Item2));
    }
}
