using System.Globalization;
using System.Linq;
using System.Text;

namespace WebAssembly.Runtime {
    /// <summary>
    /// Provides a means to convert an unrestricted WebAssembly name to a C#-compatible one.
    /// </summary>
    public static class NameCleaner {
        static bool IsPermittedIdentifierStart(char c) {
            if (c == '_')
                return true;

            return char.GetUnicodeCategory(c) switch {
                UnicodeCategory.UppercaseLetter => true,
                UnicodeCategory.LowercaseLetter => true,
                UnicodeCategory.TitlecaseLetter => true,
                UnicodeCategory.ModifierLetter => true,
                UnicodeCategory.OtherLetter => true,
                UnicodeCategory.LetterNumber => true,
                _ => false,
            };
        }

        static bool IsPermittedIdentifierPart(char c) => char.GetUnicodeCategory(c) switch {
            UnicodeCategory.UppercaseLetter => true,
            UnicodeCategory.LowercaseLetter => true,
            UnicodeCategory.TitlecaseLetter => true,
            UnicodeCategory.ModifierLetter => true,
            UnicodeCategory.OtherLetter => true,
            UnicodeCategory.LetterNumber => true,
            UnicodeCategory.NonSpacingMark => true,
            UnicodeCategory.SpacingCombiningMark => true,
            UnicodeCategory.DecimalDigitNumber => true,
            UnicodeCategory.ConnectorPunctuation => true,
            UnicodeCategory.Format => true,
            _ => false,
        };

        static bool IsPermittedIdentifier(string value) {
            if (value.Length == 0)
                return false;

            if (!IsPermittedIdentifierStart(value[0]))
                return false;

            if (value.Length == 1)
                return true;

            return value.Skip(1).All(IsPermittedIdentifierPart);
        }

        /// <summary>
        /// Ensures the provided name is compatible with C#.
        /// </summary>
        /// <param name="value">The name to convert, if necessary.</param>
        /// <returns><paramref name="value"/> or a new string if a change was needed.</returns>
        public static string CleanName(string value) {
            if (IsPermittedIdentifier(value))
                return value;

            const string prefix = "__Invalid__";

            var replacement = new StringBuilder(prefix);

            if (value.Length == 0)
                return prefix;

            static void Replace(StringBuilder replacement, char value) {
                replacement
                    .Append('_')
                    .Append(((ushort)value).ToString("X", CultureInfo.InvariantCulture))
                    .Append('_');
            }

            foreach (var c in value) {
                if (IsPermittedIdentifierPart(c)) {
                    replacement.Append(c);
                    continue;
                }

                Replace(replacement, c);
            }

            return replacement.ToString();
        }
    }

}
