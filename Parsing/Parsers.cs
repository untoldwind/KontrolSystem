namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// Expect the end of input/file to be reached
        /// </summary>
        public static Parser<char> EOF = input => {
            if (input.Available > 0) return Result.failure<char>(input, "<EOF>");
            return Result.success(input, '\0');
        };

    }
}
