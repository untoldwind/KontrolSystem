using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;

    public static class TO2ParserCommon {
        public static HashSet<string> reservedKeywords = new HashSet<string> {
            "pub", "fn", "let", "const", "if", "else", "return", "break", "continue", "while", "_", "for", "in",
            "Ok", "Err", "None", "as", "sync", "type"
        };

        public static readonly Parser<string> pubKeyword = Tag("pub").Then(Spacing1);

        public static readonly Parser<char> commaDelimiter = Char(',').Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<string> identifier = Recognize(
            Char(ch => char.IsLetter(ch) || ch == '_', "letter or _").Then(Chars0(ch => char.IsLetterOrDigit(ch) || ch == '_'))
        ).Where(s => !reservedKeywords.Contains(s), "Not a keyword").Named("<identifier>");

        public static readonly Parser<List<string>> identifierPath = Delimited1(identifier, Tag("::"));

        public static readonly Parser<TO2Type> typeRef = new Parser<TO2Type>(typeRefImpl);

        public static readonly Parser<TO2Type> typeSpec = WhiteSpaces0.Then(Char(':')).Then(WhiteSpaces0).Then(typeRef);

        public static readonly Parser<List<TO2Type>> functionTypeParamters = Char('(').Then(WhiteSpaces0).Then(DelimitedUntil(typeRef, commaDelimiter, WhiteSpaces0.Then(Char(')'))));

        public static readonly Parser<TO2Type> functionType = Seq(
            Tag("fn").Then(WhiteSpaces0).Then(functionTypeParamters), WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(typeRef)
        ).Map(items => new FunctionType(false, items.Item1, items.Item2));

        public static readonly Parser<TO2Type> tupleType = DelimitedN_M(2, null, typeRef, commaDelimiter, "<type>").Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => new TupleType(items));

        public static readonly Parser<TO2Type> recordType = Delimited1(Seq(identifier, typeSpec), commaDelimiter, "<identifier : type>").Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => new RecordTupleType(items));

        public static readonly Parser<TO2Type> typeReference = Seq(
            identifierPath,
            Opt(Delimited0(typeRef, WhiteSpaces0.Then(Char(',')).Then(WhiteSpaces0)).Between(WhiteSpaces0.Then(Char('<')).Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')))).Map(o => o.IsDefined ? o.Value : new List<TO2Type>())
        ).Map(items => BuildinType.GetBuildinType(items.Item1, items.Item2) ?? new TypeReference(items.Item1, items.Item2));

        private static readonly Parser<TO2Type> toplevelTypeRef = Seq(Alt(
            functionType,
            tupleType,
            recordType,
            typeReference
        ), Opt(Spacing0.Then(Char('[')).Then(Spacing0).Then(Char(']')))).Map(items => {
            if (items.Item2.IsDefined) return new ArrayType(items.Item1);
            return items.Item1;
        });

        private static IResult<TO2Type> typeRefImpl(IInput input) => toplevelTypeRef(input);

        public static readonly Parser<DeclarationParameter> declarationParameter = Seq(
            identifier, Opt(WhiteSpaces0.Then(Char('@')).Then(WhiteSpaces0).Then(identifier)), Opt(typeSpec)
        ).Map(items => {
            if (items.Item3.IsDefined) return new DeclarationParameter(items.Item1, items.Item2.GetOrElse(items.Item1), items.Item3.Value);
            return new DeclarationParameter(items.Item1, items.Item2.GetOrElse(items.Item1));
        });

        public static readonly Parser<DeclarationParameter> declarationParameterOrPlaceholder = Alt(
            declarationParameter,
            Char('_').Map(_ => new DeclarationParameter())
        );

        public static readonly Parser<LineComment> lineComment =
            CharsExcept0("\r\n").Map((comment, start, end) => new LineComment(comment, start, end)).Between(WhiteSpaces0.Then(Tag("//")), PeekLineEnd);

        public static readonly Parser<string> descriptionComment = Many0(CharsExcept0("\r\n").Map(s => s.Trim()).Between(WhiteSpaces0.Then(Tag("///")), PeekLineEnd)).Map(lines => String.Join("\n", lines));
    }

    public static class TO2Parser {
        public static IResult<TO2Module> TryParseModuleFile(string baseDir, string moduleFile) {
            string content = File.ReadAllText(Path.Combine(baseDir, moduleFile), Encoding.UTF8);
            IResult<TO2Module> moduleResult = TO2ParserModule.module(TO2Module.BuildName(moduleFile)).TryParse(content, moduleFile);
            if (!moduleResult.WasSuccessful) return Result.failure<TO2Module>(moduleResult.Remaining, moduleResult.Expected);

            return Result.success(moduleResult.Remaining, moduleResult.Value);
        }

        public static TO2Module ParseModuleFile(string baseDir, string moduleFile) {
            IResult<TO2Module> result = TryParseModuleFile(baseDir, moduleFile);
            if (!result.WasSuccessful) throw new ParseException(result.Position, result.Expected);
            return result.Value;
        }
    }
}
