using FormatWith.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FormatWith.Internal
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
            return FormatWith(
                formatString,
                (key, format) => new ReplacementResult(replacements.TryGetValue(key, out string value), value),
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
            return FormatWith(
                formatString,
                (key, format) => new ReplacementResult(replacements.TryGetValue(key, out object value), value),
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
                (key, format) => FromReplacementObject(key, replacementObject),
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        public static string FormatWith(
            string formatString,
            Func<string, string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            var resultBuilder = new StringBuilder(formatString.Length * 2);
            
            void ForEachToken(FormatToken token) 
                => FormatHelpers.ProcessToken(token, resultBuilder, handler, missingKeyBehaviour, fallbackReplacementValue);

            Tokenizer.Tokenize(formatString.AsSpan(), ForEachToken, openBraceChar, closeBraceChar);

            return resultBuilder.ToString();
        }

        public static FormattableString FormattableWith(
            string formatString,
            IDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormattableWith(
                formatString,
                key => new ReplacementResult(replacements.TryGetValue(key, out string value), value),
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
            return FormattableWith(
                formatString,
                key => new ReplacementResult(replacements.TryGetValue(key, out object value), value),
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
            
            // create a StringBuilder to hold the resultant output string
            // use the input hint as the initial size
            var resultBuilder = new StringBuilder(formatString.Length * 2);
            var replacementParams = new List<object>();
            int placeholderIndex = 0;

            void ForEachToken(FormatToken token)
                => FormatHelpers.ProcessTokenIntoFormattableString(token, resultBuilder, replacementParams, handler, missingKeyBehaviour, fallbackReplacementValue, ref placeholderIndex);

            Tokenizer.Tokenize(formatString.AsSpan(), ForEachToken, openBraceChar, closeBraceChar);
            
            return FormattableStringFactory.Create(resultBuilder.ToString(), replacementParams.ToArray());

        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{String}">IEnumerable&lt;string&gt;</see> that will return all format parameters used within the format string.
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

            var results = new List<string>();
            
            void ForEachToken(FormatToken token)
            {
                if (token.TokenType == TokenKind.Parameter)
                {
                    results.Add(token.Value.ToString());
                }
            }
            
            Tokenizer.Tokenize(formatString.AsSpan(), ForEachToken, openBraceChar, closeBraceChar);

            return results;
        }

        private static ReplacementResult FromReplacementObject(string key, object replacementObject)
        {
            // need to split this into accessors so we can traverse nested objects
            var members = key.Split(new[] { "." }, StringSplitOptions.None);
            if (members.Length == 1)
            {
                PropertyInfo propertyInfo = replacementObject.GetType().GetProperty(key, propertyBindingFlags);

                if (propertyInfo == null)
                {
                    return new ReplacementResult(false, null);
                }
                else
                {
                    return new ReplacementResult(true, propertyInfo.GetValue(replacementObject));
                }
            }
            else
            {
                object currentObject = replacementObject;

                foreach (var member in members)
                {
                    PropertyInfo propertyInfo = currentObject.GetType().GetProperty(member, propertyBindingFlags);

                    if (propertyInfo == null)
                    {
                        return new ReplacementResult(false, null);
                    }
                    else
                    {
                        currentObject = propertyInfo.GetValue(currentObject);
                    }
                }

                return new ReplacementResult(true, currentObject);
            }
        }
    }
}
