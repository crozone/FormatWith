using System;
using System.Collections.Generic;
using FormatWith.Internal;
using System.Collections;
using static FormatWith.Internal.FormatWithMethods;

namespace FormatWith
{
    /// <summary>
    /// The string extensions provided by FormatWith for string formatting.
    /// </summary>
    public static class StringExtensions
    {
        #region FormatWith Overloads
        /// <summary>
        /// Formats a string with the values given by the properties on an input object.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacementObject">The object whose properties should be injected in the string</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(
            this string formatString,
            object replacementObject)
        {
            // wrap the type object in a wrapper Dictionary class that exposes the properties as dictionary keys via reflection
            return FormatWithMethods.FormatWith(formatString.AsSpan(), replacementObject);
        }

        /// <summary>
        /// Formats a string with the values given by the properties on an input object.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacementObject">The object whose properties should be injected in the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(
            this string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormatWith(
                formatString.AsSpan(),
                replacementObject,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Formats a string with the values of the dictionary.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith<T>(
            this string formatString,
            IReadOnlyDictionary<string, T> replacements)
        {
            return FormatWithMethods.FormatWith(formatString.AsSpan(), replacements);
        }

        /// <summary>
        /// Formats a string with the values of the dictionary.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith<T>(
            this string formatString,
            IReadOnlyDictionary<string, T> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormatWith(
                formatString.AsSpan(),
                replacements,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Formats a string, using a handler function to provide the value
        /// of each parameter.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="handler">A handler function that transforms each parameter into a <see cref="ReplacementResult"/></param>
        /// <returns>The formatted string</returns>
        public static string FormatWith<T>(
            this string formatString,
            Func<string, string, ReplacementResult<T>> handler)
        {
            return FormatWithMethods.FormatWith(formatString.AsSpan(), handler);
        }

        /// <summary>
        /// Formats a string, using a handler function to provide the value
        /// of each parameter.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="handler">A handler function that transforms each parameter into a <see cref="ReplacementResult"/></param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that cannot be replaced by the handler</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this object is used as a fallback replacement value.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith<T>(
            this string formatString,
            Func<string, string, ReplacementResult<T>> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormatWith(
                formatString.AsSpan(),
                handler,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        // TODO: The new Span<T> based extension method.
        // TODO: XML Docstrings
        public static string FormatWith(
            this ReadOnlySpan<char> formatString,
            HandlerAction handlerAction,
            MissingKeyBehaviour missingKeyBehaviour,
            FallbackAction fallbackReplacementAction = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormatWith(
                formatString,
                handlerAction,
                missingKeyBehaviour,
                fallbackReplacementAction,
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
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith(this string formatString, object replacementObject)
        {
            // wrap the type object in a wrapper Dictionary class that exposes the properties as dictionary keys via reflection
            return FormatWithMethods.FormattableWith(formatString, replacementObject);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacementObject">The object whose properties should be injected in the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith(
            this string formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormattableWith(
                formatString,
                replacementObject,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith<T>(this string formatString, IReadOnlyDictionary<string, T> replacements)
        {
            return FormatWithMethods.FormattableWith(formatString, replacements);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith<T>(
            this string formatString,
            IReadOnlyDictionary<string, T> replacements,
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

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith(this string formatString, IReadOnlyDictionary<string, object> replacements)
        {
            return FormatWithMethods.FormattableWith(formatString, replacements);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="replacements">An <see cref="IReadOnlyDictionary"/> with keys and values to inject into the string</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith(
            this string formatString,
            IReadOnlyDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
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

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="handler">A handler function that transforms each parameter into a <see cref="ReplacementResult"/></param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith<T>(
            this string formatString,
            Func<string, ReplacementResult<T>> handler)
        {
            return FormatWithMethods.FormattableWith(formatString, handler);
        }

        /// <summary>
        /// Produces a <see cref="FormattableString"/> representing the input format string.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="handler">A handler function that transforms each parameter into a <see cref="ReplacementResult"/></param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that cannot be replaced by the handler</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this object is used as a fallback replacement value.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>The resultant <see cref="FormattableString"/></returns>
        public static FormattableString FormattableWith<T>(
            this string formatString,
            Func<string, ReplacementResult<T>> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.FormattableWith(
                formatString,
                handler,
                missingKeyBehaviour,
                fallbackReplacementValue,
                openBraceChar,
                closeBraceChar);
        }

        #endregion

        /// <summary>
        /// Gets an <see cref="IEnumerable{String}"/> that will return all format parameters used within the format string.
        /// </summary>
        /// <param name="formatString">The format string to be parsed</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFormatParameters(
            this string formatString,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormatWithMethods.GetFormatParameters(formatString, openBraceChar, closeBraceChar);
        }
    }
}
