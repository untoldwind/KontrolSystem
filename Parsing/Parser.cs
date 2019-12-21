using System;
using System.Collections.Generic;

namespace KontrolSystem.Parsing {
    public delegate IResult<T> Parser<out T>(IInput input);

    public static class ParserExtensions {
        public static IResult<T> TryParse<T>(this Parser<T> parser, string input, string sourceName = "<inline>") => parser(new StringInput(input, sourceName));

        public static T Parse<T>(this Parser<T> parser, string input, string sourceName = "<inline>") {
            IResult<T> result = parser(new StringInput(input, sourceName));

            if (!result.WasSuccessful) throw new ParseException(result.Position, result.Expected);

            return result.Value;
        }

        public static Parser<T> Named<T>(this Parser<T> parser, string expected) => input => {
            IResult<T> result = parser(input);
            if (!result.WasSuccessful) return Result.failure<T>(result.Remaining, expected.Yield());
            return result;
        };
    }

    public class ParseException : System.Exception {
        public readonly Position position;
        public readonly IEnumerable<string> expected;

        public ParseException(Position _position, IEnumerable<string> _expected) : base($"{_position}: Expected {String.Join(" or ", _expected)}") {
            position = _position;
            expected = _expected;
        }
    }

    public static class ObjectExt {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item) {
            yield return item;
        }
    }
}
