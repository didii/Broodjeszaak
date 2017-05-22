using System.Linq;

namespace BroodjeszaakLib {
    /// <summary>
    ///     Some extensions used for string manipulation
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        ///     Converts a string to its title case representation. This only set the first character to uppercase.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string source) {
            if (string.IsNullOrEmpty(source))
                return source;
            var chars = source.ToArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
