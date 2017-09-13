using FormatWith.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FormatWith
{
    internal static class FormatWithMethods
    {
        public static string FormatWith(
            string formatString,
            IDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWith(formatString, key =>
            {
                return new ReplacementResult
                {
                    Success = replacements.TryGetValue(key, out string value),
                    Value = value
                };
            },
            missingKeyBehaviour,
            fallbackReplacementValue,
            openBraceChar,
            closeBraceChar);
        }

        public static string FormatWith(
            string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWith(formatString, key =>
            {
                return new ReplacementResult
                {
                    Success = replacements.TryGetValue(key, out object value),
                    Value = value
                };
            },
            missingKeyBehaviour,
            fallbackReplacementValue,
            openBraceChar,
            closeBraceChar);
        }

        private static BindingFlags propertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public static string FormatWith(
            string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (replacementObject == null) throw new ArgumentNullException(nameof(replacementObject));

            return FormatWith(formatString,
                key => FromReplacementObject(key, replacementObject),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        public static string FormatWith(
            string formatString,
            Func<string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            // get the parameters from the format string
            IEnumerable<FormatToken> tokens = FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar);
            return FormatHelpers.ProcessTokens(tokens, handler, missingKeyBehaviour, fallbackReplacementValue, formatString.Length * 2);
        }

        public static FormattableString FormattableWith(
            string formatString,
            IDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormattableWith(formatString, key =>
            {
                return new ReplacementResult
                {
                    Success = replacements.TryGetValue(key, out string value),
                    Value = value
                };
            },
            missingKeyBehaviour,
            fallbackReplacementValue,
            openBraceChar,
            closeBraceChar);
        }

        public static FormattableString FormattableWith(
            string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormattableWith(formatString, key =>
            {
                return new ReplacementResult
                {
                    Success = replacements.TryGetValue(key, out object value),
                    Value = value
                };
            },
            missingKeyBehaviour,
            fallbackReplacementValue,
            openBraceChar,
            closeBraceChar);
        }

        public static FormattableString FormattableWith(
            string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormattableWith(formatString,
                key => FromReplacementObject(key, replacementObject),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        public static FormattableString FormattableWith(
            string formatString,
            Func<string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // get the parameters from the format string
            IEnumerable<FormatToken> tokens = FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar);
            return FormatHelpers.ProcessTokensIntoFormattableString(tokens, handler, missingKeyBehaviour, fallbackReplacementValue, formatString.Length * 2);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{string}"/> that will return all format parameters used within the format string.
        /// </summary>
        /// <param name="formatString">The format string to be parsed</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFormatParameters(
            string formatString,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar)
                .Where(t => t.TokenType == TokenType.Parameter)
                .Select(pt => pt.Value);
        }

        private static ReplacementResult FromReplacementObject(string key, object replacementObject)
        {
            PropertyInfo propertyInfo = replacementObject.GetType().GetProperty(key, propertyBindingFlags);
            if (propertyInfo == null)
            {
                return new ReplacementResult()
                {
                    Success = false,
                    Value = null
                };
            }
            else
            {
                return new ReplacementResult
                {
                    Success = true,
                    Value = propertyInfo.GetValue(replacementObject)
                };
            }
        }
    }
}
