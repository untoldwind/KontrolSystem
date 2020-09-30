using System;
using System.Collections.Generic;

namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// Parse n to m items.
        /// </summary>
        public static Parser<List<T>> ManyN_M<T>(int? minCount, int? maxCount, Parser<T> itemParser,
            string description = "items") => input => {
            IInput remaining = input;
            List<T> result = new List<T>();
            IResult<T> itemResult = itemParser(input);

            while (itemResult.WasSuccessful) {
                if (remaining.Position == itemResult.Remaining.Position) break;

                result.Add(itemResult.Value);
                remaining = itemResult.Remaining;

                itemResult = itemParser(remaining);
            }

            if (minCount.HasValue && result.Count < minCount)
                return Result.Failure<List<T>>(input, $"Expected at least {minCount} {description}");
            if (maxCount.HasValue && result.Count > maxCount)
                return Result.Failure<List<T>>(input, $"Expected at most {minCount} {description}");

            return Result.Success(remaining, result);
        };

        /// <summary>
        /// Parser zero or more items.
        /// </summary>
        public static Parser<List<T>> Many0<T>(Parser<T> itemParser, string description = "items") =>
            ManyN_M(null, null, itemParser, description);

        /// <summary>
        /// Parser one or more items.
        /// </summary>
        public static Parser<List<T>> Many1<T>(Parser<T> itemParser, string description = "items") =>
            ManyN_M(1, null, itemParser, description);

        /// <summary>
        /// Parser n to m items separated by a delimiter.
        /// </summary>
        public static Parser<List<T>> DelimitedN_M<T, TD>(int? minCount, int? maxCount, Parser<T> itemParser,
            Parser<TD> delimiter, string description = "items") => input => {
            IInput remaining = input;
            List<T> result = new List<T>();
            IResult<T> itemResult = itemParser(input);

            while (itemResult.WasSuccessful) {
                if (remaining.Position == itemResult.Remaining.Position) break;

                result.Add(itemResult.Value);
                remaining = itemResult.Remaining;

                IResult<TD> delimiterResult = delimiter(remaining);
                if (!delimiterResult.WasSuccessful) break;

                itemResult = itemParser(delimiterResult.Remaining);
            }

            if (minCount.HasValue && result.Count < minCount)
                return Result.Failure<List<T>>(input, $"Expected at least {minCount} {description}");
            if (maxCount.HasValue && result.Count > maxCount)
                return Result.Failure<List<T>>(input, $"Expected at most {minCount} {description}");

            return Result.Success(remaining, result);
        };

        /// <summary>
        /// Parser zero or more items separated by a delimiter.
        /// </summary>
        public static Parser<List<T>> Delimited0<T, TD>(Parser<T> itemParser, Parser<TD> delimiter,
            string description = "items") => DelimitedN_M(null, null, itemParser, delimiter, description);

        /// <summary>
        /// Parser one or more items separated by a delimiter.
        /// </summary>
        public static Parser<List<T>> Delimited1<T, TD>(Parser<T> itemParser, Parser<TD> delimiter,
            string description = "items") => DelimitedN_M(1, null, itemParser, delimiter, description);

        /// <summary>
        /// Parse any number of items until an end condition is met.
        /// </summary>
        public static Parser<List<T>> DelimitedUntil<T, TD, TE>(Parser<T> itemParser, Parser<TD> delimiter, Parser<TE> end,
            string description = "item") => input => {
            IInput remaining = input;
            List<T> result = new List<T>();
            IResult<TE> endResult = end(remaining);

            if (endResult.WasSuccessful) return Result.Success(endResult.Remaining, result);

            while (remaining.Available > 0) {
                IResult<T> itemResult = itemParser(remaining);
                if (!itemResult.WasSuccessful)
                    return Result.Failure<List<T>>(itemResult.Remaining, itemResult.Expected);
                if (remaining.Position == itemResult.Remaining.Position)
                    return Result.Failure<List<T>>(remaining, description);

                result.Add(itemResult.Value);
                remaining = itemResult.Remaining;

                endResult = end(remaining);
                if (endResult.WasSuccessful) return Result.Success(endResult.Remaining, result);

                IResult<TD> delimiterResult = delimiter(remaining);
                if (!delimiterResult.WasSuccessful) return Result.Failure<List<T>>(remaining, delimiterResult.Expected);

                remaining = delimiterResult.Remaining;

                endResult = end(remaining);
                if (endResult.WasSuccessful) return Result.Success(endResult.Remaining, result);
            }

            return Result.Failure<List<T>>(remaining, endResult.Expected);
        };

        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        public static Parser<T> Chain<T, TOp>(Parser<T> operantParser, Parser<TOp> opParser,
            Func<T, TOp, T, Position, Position, T> apply) {
            Parser<(TOp, T)> restParser = Seq(opParser, operantParser);

            return input => {
                IResult<T> firstResult = operantParser(input);

                if (!firstResult.WasSuccessful) return firstResult;

                IInput remaining = firstResult.Remaining;
                T result = firstResult.Value;

                IResult<(TOp, T)> restResult = restParser(remaining);

                while (restResult.WasSuccessful) {
                    if (remaining.Position == restResult.Remaining.Position)
                        break;

                    var (op, operant) = restResult.Value;
                    result = apply(result, op, operant, remaining.Position, restResult.Position);
                    remaining = restResult.Remaining;

                    restResult = restParser(remaining);
                }

                return Result.Success(remaining, result);
            };
        }

        /// <summary>
        /// Fold an initial parser with zero or more successors.
        /// </summary>
        public static Parser<T> Fold0<T, TS>(this Parser<T> initial, Parser<TS> suffix,
            Func<T, TS, Position, Position, T> combine) => input => {
            IResult<T> result = initial(input);
            if (!result.WasSuccessful) return result;

            IResult<TS> suffixResult = suffix(result.Remaining);
            while (suffixResult.WasSuccessful) {
                if (suffixResult.Position == result.Position) break;

                result = Result.Success(suffixResult.Remaining,
                    combine(result.Value, suffixResult.Value, result.Position, suffixResult.Position));
                suffixResult = suffix(result.Remaining);
            }

            return result;
        };
    }
}
