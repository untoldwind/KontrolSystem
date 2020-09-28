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
        public static readonly Parser<string> UseKeyword = Tag("use").Then(Spacing1);

        public static readonly Parser<String> TypeKeyword = Tag("type").Then(Spacing1);

        public static readonly Parser<string> FormKeyword = Spacing1.Then(Tag("from")).Then(Spacing1);

        public static readonly Parser<string> AsKeyword = Spacing1.Then(Tag("as")).Then(Spacing1);

        public static readonly Parser<List<string>> UseNames = Alt(
            Char('*').Map(_ => (List<string>)null),
            Delimited1(Identifier, Char(',').Between(WhiteSpaces0, WhiteSpaces0)).Between(Char('{').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('}')))
        );

        public static readonly Parser<UseDeclaration> UseNamesDeclaration = Seq(
            UseKeyword.Then(UseNames), FormKeyword.Then(IdentifierPath)
        ).Map((decl, start, end) => new UseDeclaration(decl.Item1, decl.Item2, start, end));

        public static readonly Parser<UseDeclaration> UseAliasDeclaration = Seq(
            UseKeyword.Then(IdentifierPath), AsKeyword.Then(Identifier)
        ).Map((decl, start, end) => new UseDeclaration(decl.Item1, decl.Item2, start, end));

        public static readonly Parser<TypeAlias> TypeAlias = Seq(
             DescriptionComment, WhiteSpaces0.Then(Opt(PubKeyword)), TypeKeyword.Then(Identifier), WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(TypeRef)
        ).Map((items, start, end) => new TypeAlias(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, start, end));

        public static readonly Parser<ConstDeclaration> ConstDeclaration = Seq(
            DescriptionComment, Opt(PubKeyword), Tag("const").Then(WhiteSpaces1).Then(Identifier), TypeSpec, WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Expression)
        ).Map((items, start, end) => new ConstDeclaration(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, items.Item5, start, end));

        public static readonly Parser<IModuleItem> ModuleItem = Alt<IModuleItem>(
            UseNamesDeclaration,
            UseAliasDeclaration,
            functionDeclaration,
            TypeAlias,
            ConstDeclaration,
            LineComment
        );

        public static readonly Parser<List<IModuleItem>> ModuleItems = WhiteSpaces0.Then(DelimitedUntil(ModuleItem, WhiteSpaces1, EOF));

        public static Parser<TO2Module> Module(string moduleName) => Seq(WhiteSpaces0.Then(DescriptionComment), ModuleItems).Map(items => new TO2Module(moduleName, items.Item1, items.Item2));
    }
}
