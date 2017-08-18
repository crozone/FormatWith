using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatWith.Internal;
using System.Runtime.CompilerServices;

namespace FormatWith {
    public static class StringExtensions {
        #region FormatWith Overloads
        /// <summary>
        /// Formats a string with the values given by the properties on an input object.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacementObject">The object whose properties should be injected in the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static string FormatWith(
            this string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // wrap the type object in a wrapper Dictionary class that exposes the properties as dictionary keys via reflection
            return FormatWithMethods.FormatWith(
                formatString,
                new DictionaryTypeWrapper(replacementObject),
                missingKeyBehaviour, fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Formats a string with the values of the dictionary.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static string FormatWith(
            this string formatString,
            IDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // wrap the IDictionary<string, string> in a wrapper Dictionary class that casts the values to objects as needed
            return FormatWithMethods.FormatWith(
                formatString, new DictionaryStringToObjectWrapper<string, string>(replacements),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Formats a string with the values of the dictionary.
        /// The string representation of each object value in the dictionary is used as the format parameter.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static string FormatWith(
            this string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormatWith(
                formatString, replacements,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        #endregion

        #region FormattableWith Overloads
        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacementObject">The object whose properties should be injected in the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A FormattableString representing the  string with dictionary keys replaced by (formatted) key values</returns>
        public static FormattableString FormattableWith(
            this string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // wrap the type object in a wrapper Dictionary class that exposes the properties as dictionary keys via reflection
            return FormatWithMethods.FormattableWith(
                formatString,
                new DictionaryTypeWrapper(replacementObject),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static FormattableString FormattableWith(
            this string formatString,
            IDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // wrap the IDictionary<string, string> in a wrapper Dictionary class that casts the values to objects as needed
            return FormatWithMethods.FormattableWith(
                formatString,
                new DictionaryStringToObjectWrapper<string, string>(replacements),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values</returns>
        public static FormattableString FormattableWith(
            this string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormattableWith(
                formatString,
                replacements,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        #endregion

        /// <summary>
        /// Gets an <see cref="IEnumerable{string}"/> that will return all format parameters used within the format string.
        /// </summary>
        /// <param name="formatString">The format string to be parsed</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFormatParameters(
            this string formatString,
            char openBraceChar = '{',
            char closeBraceChar = '}') {
            return FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar)
                .Where(t => t.TokenType == TokenType.Parameter)
                .Select(pt => pt.Text);
        }
    }
}
