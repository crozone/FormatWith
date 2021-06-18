using FormatWith.Internal;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FormatWith.Internal
{
    internal static class FormatWithMethods
    {
        public static string FormatWith(
            ReadOnlySpan<char> formatString,
            IReadOnlyDictionary<string, string> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                if (replacements.TryGetValue(key.ToString(), out string value))
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(value.AsSpan());
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", value).AsSpan());
                    }
                }
            }

            void fallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, fallbackAction, openBraceChar, closeBraceChar);
        }

        public static string FormatWith(
            ReadOnlySpan<char> formatString,
            IReadOnlyDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                if (replacements.TryGetValue(key.ToString(), out object value))
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(value.ToString().AsSpan());
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", value).AsSpan());
                    }
                }
            }

            void FallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, FallbackAction, openBraceChar, closeBraceChar);
        }

        public static string FormatWith(
            ReadOnlySpan<char> formatString,
            object replacementObject,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (replacementObject == null) throw new ArgumentNullException(nameof(replacementObject));
            if (formatString.Length == 0) return string.Empty;

            void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                if (TryGetPropertyFromObject(key.ToString(), replacementObject, out object value))
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(value.ToString().AsSpan());
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", value).AsSpan());
                    }
                }
            }

            void FallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, FallbackAction, openBraceChar, closeBraceChar);
        }

        public static string FormatWith(
            ReadOnlySpan<char> formatString,
            Func<string, string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                var replacementResult = handler(key.ToString(), format.ToString());

                if (replacementResult.Success)
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(replacementResult.Value.ToString().AsSpan());
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", replacementResult.Value).AsSpan());
                    }
                }
            }

            void FallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, FallbackAction, openBraceChar, closeBraceChar);
        }

        //
        // Span based versions of FormatWith
        //

        public static string FormatWith(
            ReadOnlySpan<char> formatString,
            HandlerAction handlerAction,
            MissingKeyBehaviour missingKeyBehaviour,
            FallbackAction fallbackReplacementAction = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            StringBuilder stringBuilder = GetStringBuilder(formatString.Length * 2);

            void ResultAction(ReadOnlySpan<char> value) => stringBuilder.Append(value);

            FormatWith(formatString, handlerAction, ResultAction, missingKeyBehaviour, fallbackReplacementAction, openBraceChar, closeBraceChar);

            return stringBuilder.ToString();
        }

        public static void FormatWith(
            ReadOnlySpan<char> formatString,
            HandlerAction handlerAction,
            ResultAction resultAction,
            MissingKeyBehaviour missingKeyBehaviour,
            FallbackAction fallbackReplacementAction = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return;

            // TODO: We can avoid internal delegate/closure allocations once function pointers are implemented:
            // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
            void ForEachToken(FormatToken token) => FormatHelpers.ProcessToken(token, handlerAction, resultAction, missingKeyBehaviour, fallbackReplacementAction);

            Tokenizer.Tokenize(formatString, ForEachToken, openBraceChar, closeBraceChar);
        }


        private static readonly ThreadLocal<WeakReference<StringBuilder>> threadLocalStringBuilder =
            new ThreadLocal<WeakReference<StringBuilder>>(() => new WeakReference<StringBuilder>(new StringBuilder(), false));

        /// <summary>
        /// Returns a StringBuilder of at least the given capacity.
        /// Caches the StringBuilder in a weakly referenced ThreadLocal for efficiency.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        private static StringBuilder GetStringBuilder(int capacity)
        {
            var weakRef = threadLocalStringBuilder.Value;
            if (weakRef.TryGetTarget(out StringBuilder stringBuilder))
            {
                stringBuilder.Clear();
                stringBuilder.EnsureCapacity(capacity);
            }
            else
            {
                stringBuilder = new StringBuilder(capacity);
                weakRef.SetTarget(stringBuilder);
            }

            return stringBuilder;
        }

        //
        // FormattableWith overloads
        //

        public static FormattableString FormattableWith(
            string formatString,
            IReadOnlyDictionary<string, string> replacements,
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
            IReadOnlyDictionary<string, object> replacements,
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
                if (token.TokenKind == TokenKind.Parameter)
                {
                    results.Add(token.Value.ToString());
                }
            }
            
            Tokenizer.Tokenize(formatString.AsSpan(), ForEachToken, openBraceChar, closeBraceChar);

            return results;
        }

        private static readonly BindingFlags propertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

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

        private static bool TryGetPropertyFromObject(string key, object replacementObject, out object value)
        {
            // need to split this into accessors so we can traverse nested objects
            var members = key.Split(new[] { "." }, StringSplitOptions.None);
            if (members.Length == 1)
            {
                PropertyInfo propertyInfo = replacementObject.GetType().GetProperty(key, propertyBindingFlags);

                if (propertyInfo == null)
                {
                    value = null;
                    return false;
                }
                else
                {
                    value = propertyInfo.GetValue(replacementObject);
                    return true;
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
                        value = null;
                        return false;
                    }
                    else
                    {
                        currentObject = propertyInfo.GetValue(currentObject);
                    }
                }

                value = currentObject;
                return true;
            }
        }
    }
}
