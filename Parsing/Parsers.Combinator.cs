using System;
namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// A parser that is always successful and does not consume any input.
        /// </summary>
        public static Parser<T> Value<T>(T value) => input => Result.success(input, value);

        /// <summary>
        /// Construct a parser that indicates that the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        public static Parser<IOption<T>> Opt<T>(Parser<T> parser) => input => {
            IResult<T> result = parser(input);

            if (!result.WasSuccessful) return Result.success(input, Option.none<T>());

            return Result.success(result.Remaining, Option.some(result.Value));
        };

        /// <summary>
        /// If child parser was successful, return consumed input.
        /// </summary>
        public static Parser<string> Recognize<T>(Parser<T> parser) => input => {
            IResult<T> result = parser(input);
            if (!result.WasSuccessful) return Result.failure<string>(result.Remaining, result.Expected);
            return Result.success(result.Remaining, input.Take(result.Remaining.Position.position - input.Position.position));
        };

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> convert) => input => parser(input).Select(s => Result.success(s.Remaining, convert(s.Value)));

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain with positions.
        /// </summary>
        public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, Position, Position, U> convert) => input => parser(input).Select(s => Result.success(s.Remaining, convert(s.Value, input.Position, s.Position)));

        /// <summary>
        /// Filter the result of a parser by a predicate.
        /// </summary>
        public static Parser<T> Where<T>(this Parser<T> parser, Predicate<T> predicate, string deTO2ion) => input => {
            IResult<T> result = parser(input);
            if (!result.WasSuccessful) return result;
            if (!predicate(result.Value)) return Result.failure<T>(result.Remaining, deTO2ion);
            return result;
        };
    }
}
