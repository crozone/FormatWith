using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FormatWith.Internal
{
    internal static class FormatWithMethods
    {
        public static string FormatWith<T>(
            ReadOnlySpan<char> formatString,
            IReadOnlyDictionary<string, T> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            bool HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                if (replacements.TryGetValue(key.ToString(), out T value))
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(value.ToString().AsSpan());
                        return true;
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", value).AsSpan());
                        return true;
                    }
                }

                return false;
            }

            void fallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, fallbackAction, openBraceChar, closeBraceChar);
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

            bool HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                if (TryGetPropertyFromObject(key.ToString(), replacementObject, out object value))
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(value.ToString().AsSpan());
                        return true;
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", value).AsSpan());
                        return true;
                    }
                }
                return false;
            }

            void FallbackAction(ResultAction resultAction)
            {
                resultAction(fallbackReplacementValue.ToString().AsSpan());
            }

            return FormatWith(formatString, HandlerAction, missingKeyBehaviour, FallbackAction, openBraceChar, closeBraceChar);
        }

        public static string FormatWith<T>(
            ReadOnlySpan<char> formatString,
            Func<string, string, ReplacementResult<T>> handler,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;

            bool HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction)
            {
                var replacementResult = handler(key.ToString(), format.ToString());

                if (replacementResult.Success)
                {
                    if (format == null || format.IsEmpty || format.IsWhiteSpace())
                    {
                        resultAction(replacementResult.Value.ToString().AsSpan());
                        return true;
                    }
                    else
                    {
                        resultAction(string.Format("{0:" + format.ToString() + "}", replacementResult.Value).AsSpan());
                        return true;
                    }
                }

                return false;
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

            (StringBuilder stringBuilder, ResultAction appenderAction) = StringBuilderCache.GetStringBuilder(formatString.Length * 2);
            FormatWith(formatString, handlerAction, appenderAction, missingKeyBehaviour, fallbackReplacementAction, openBraceChar, closeBraceChar);

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

            // A note on delegates:
            //
            // We currently have three delegate allocations per FormatWith call, the handlerAction, the resultAction, and the Tokenizer callback.
            // This only happens once per FormatWith call but it is less than ideal if we're attempting to be as low allocation as possible.
            //
            // NOTE: Need to investigate if any of these are cached by the compiler.
            //
            // 1. A lambda expression which doesn't capture any variables is cached statically
            // 2. A lambda expression which only captures "this" could be captured on a per-instance basis, but isn't
            // 3. A lambda expression which captures a local variable can't be cached
            //
            // TODO: These are some potential workarounds to eliminate these allocations:
            //
            // 1. SOLVED. We cache the appender delegate next to the StringBuilder, so the appender delegate doesn't need to be recreated each call to FormatWith
            //
            // 2. ForEachToken could be eliminated by moving all possible variations of the callback into the Tokenizer code directly, and then passing in an enum
            // and all state variables, keeping everything on the stack.
            //
            // 3. handlerAction could be eliminated by moving the internal variations into ProcessToken along with an enum and all state variables.
            //
            // None of these are ideal but will be necessary unless the compiler/JIT gets a lot smarter about inlining and eliminates these delegates in internal cases.
            //
            // Function pointers, once implemented, may provide a solution to delegate allocation (albeit with unsafe code):
            // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
            //
            // See also:
            // https://devblogs.microsoft.com/premier-developer/dissecting-the-local-functions-in-c-7/
            //

            void ForEachToken(FormatToken token) => FormatHelpers.ProcessToken(token, handlerAction, resultAction, missingKeyBehaviour, fallbackReplacementAction);

            Tokenizer.Tokenize(formatString, ForEachToken, openBraceChar, closeBraceChar);
        }

        //
        // FormattableWith overloads
        //

        public static FormattableString FormattableWith<T>(
            string formatString,
            IReadOnlyDictionary<string, T> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            string fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            return FormattableWith(
                formatString,
                key => new ReplacementResult<T>(replacements.TryGetValue(key, out T value), value),
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

        public static FormattableString FormattableWith<T>(
            string formatString,
            Func<string, ReplacementResult<T>> handler,
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

        private static ReplacementResult<object> FromReplacementObject(string key, object replacementObject)
        {
            if (TryGetPropertyFromObject(key, replacementObject, out object value))
            {
                return new ReplacementResult<object>(true, value);
            }
            else
            {
                return new ReplacementResult<object>(false, null);
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
