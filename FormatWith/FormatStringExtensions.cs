using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatWith.Internal;

namespace FormatWith {
    public static class FormatStringExtensions {
        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching object properties.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static string FormatWith(this string formatString, object injectionObject, MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException, string fallbackReplacementValue = null, char openBraceChar = '{', char closeBraceChar = '}') {
            // wrap the type object in a wrapper Dictionary class that exposes the properties as dictionary keys via reflection
            return formatString.FormatWith(new DictionaryTypeWrapper(injectionObject), missingKeyBehaviour, fallbackReplacementValue, openBraceChar, closeBraceChar);
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
        public static string FormatWith(this string formatString, IDictionary<string, string> replacements, MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException, string fallbackReplacementValue = null, char openBraceChar = '{', char closeBraceChar = '}') {
            // wrap the IDictionary<string, string> in a wrapper Dictionary class that casts the values to objects as needed
            return formatString.FormatWith(new DictionaryStringToObjectWrapper<string, string>(replacements), missingKeyBehaviour, fallbackReplacementValue, openBraceChar, closeBraceChar);
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
            IEnumerable<FormatToken> tokens = FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar);
            return FormatHelpers.ProcessTokens(tokens, replacements, missingKeyBehaviour, fallbackReplacementValue);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{string}"/> that will return all format parameters used within the format string.
        /// </summary>
        /// <param name="formatString">The format string to be parsed</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFormatParameters(this string formatString, char openBraceChar = '{', char closeBraceChar = '}') {
            return FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar)
                .Where(t => t.TokenType == TokenType.Parameter)
                .Select(pt => pt.Text);
        }
    }
}
