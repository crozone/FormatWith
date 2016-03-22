using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FormatWith.FormatHelpers;
using static FormatWith.ObjectHelpers;

namespace FormatWith {
    public static class FormatStringExtensions {
        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching object properties.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) key values</returns>
        public static string FormatWith(this string formatString, object injectionObject, MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException, string fallbackReplacementValue = null, char openBraceChar = '{', char closeBraceChar = '}') {
            return formatString.FormatWith(GetPropertiesDictionary(injectionObject), missingKeyBehaviour, fallbackReplacementValue, openBraceChar, closeBraceChar);
        }

        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching dictionary entries.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static string FormatWith(this string formatString, IDictionary<string, object> replacements, MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException, string fallbackReplacementValue = null, char openBraceChar = '{', char closeBraceChar = '}') {

            // get the parameters from the format string
            var tokens = Tokenize(formatString, openBraceChar, closeBraceChar);
            return ProcessTokens(tokens, replacements, missingKeyBehaviour, fallbackReplacementValue);
        }
    }
}
