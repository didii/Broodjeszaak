using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroodjeszaakLib {
    public static class StringExtensions {

        public static string ToTitleCase(this string source) {
            if (string.IsNullOrEmpty(source))
                return source;
            var chars = source.ToArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
