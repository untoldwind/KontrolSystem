using System;
using System.Globalization;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;

    public static class TO2ParserLiterals {
        private static readonly Parser<char> doubleQuote = Char('"');

        private static readonly Parser<char> escapedStringChar = Alt(
                    CharExcept("\\\"\r\n"),
                    Tag("\\\"").Map(_ => '"'),
                    Tag("\\t").Map(_ => '\t'),
                    Tag("\\n").Map(_ => '\n'),
                    Tag("\\r").Map(_ => '\r')
                );

        public static readonly Parser<LiteralString> literalString = Many0(escapedStringChar).Between(doubleQuote, doubleQuote).Map((chars, start, end) => new LiteralString(chars.ToArray(), start, end)).Named("<string>");

        public static readonly Parser<int> basePrefix = Alt(
            Tag("0x").Map(_ => 16),
            Tag("0o").Map(_ => 8),
            Tag("0b").Map(_ => 2),
            Value(10)
        );

        public static readonly Parser<LiteralInt> literalInt = Seq(
                Opt(Char('-')), basePrefix, Recognize(Digits1.Then(Chars0(ch => char.IsDigit(ch) || ch == '_')))
                ).Map((items, start, end) => new LiteralInt(items.Item1.IsDefined ? -Convert.ToInt64(items.Item3.Replace("_", ""), items.Item2) : Convert.ToInt64(items.Item3.Replace("_", ""), items.Item2), start, end)).Named("<integer>");

        private static readonly Parser<string> exponentSuffix = OneOf("eE").Then(Opt(OneOf("+-"))).Then(Digits1);

        public static readonly Parser<LiteralFloat> literalFloat = Recognize(
                    Opt(OneOf("+-")).Then(Alt(
                                              Terminated(Digits0.Then(Char('.')).Then(Digits1), Opt(exponentSuffix)),
                                              Digits1.Then(exponentSuffix)
                                          ))
                ).Map((digits, start, end) => new LiteralFloat(Convert.ToDouble(digits, CultureInfo.InvariantCulture), start, end)).Named("<float>");

        public static readonly Parser<LiteralBool> literalBool = Alt(
                    Tag("true").Map((_, start, end) => new LiteralBool(true, start, end)),
                    Tag("false").Map((_, start, end) => new LiteralBool(false, start, end))
                );
    }
}
