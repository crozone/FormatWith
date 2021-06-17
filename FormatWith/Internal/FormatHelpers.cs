using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FormatWith.Internal
{
    /// <summary>
    /// Contains all string processing and tokenizing methods for FormatWith
    /// </summary>
    internal static class FormatHelpers
    {
        /// <summary>
        /// Processes a token into its resulting string.
        /// </summary>
        /// <param name="token">Current token to process</param>
        /// <param name="resultBuilder">The StringBuilder which will contain the result</param>
        /// <param name="handler">The function used to perform the replacements on the format tokens</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <returns>The processed result of joining the tokens with the replacement dictionary.</returns>
        public static void ProcessToken(
            FormatToken token,
            StringBuilder resultBuilder,
            Func<string, string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour,
            object fallbackReplacementValue)
        {
            if (token.TokenType == TokenType.Text)
            {
                // token is a text token
                // add the token to the result string builder
                resultBuilder.Append(token.Raw);
            }
            else if (token.TokenType == TokenType.Parameter)
            {
                // token is a parameter token
                // perform parameter logic now.
                // TODO: See about removing these ToString calls on tokenKey and format
                string tokenKey = token.Value.ToString();
                string format = null;
                var separatorIdx = tokenKey.IndexOf(':');
                if (separatorIdx > -1)
                {
                    tokenKey = token.Value.Slice(0, separatorIdx).ToString();
                    format = token.Value.Slice(separatorIdx + 1).ToString();
                }

                // append the replacement for this parameter
                ReplacementResult replacementResult = handler(tokenKey, format);

                if (replacementResult.Success)
                {
                    // the key exists, add the replacement value
                    // this does nothing if replacement value is null
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        resultBuilder.Append(replacementResult.Value);
                    }
                    else
                    {
                        resultBuilder.AppendFormat("{0:" + format + "}", replacementResult.Value);
                    }
                }
                else
                {
                    // the key does not exist, handle this using the missing key behaviour specified.
                    switch (missingKeyBehaviour)
                    {
                        case MissingKeyBehaviour.ThrowException:
                            // the key was not found as a possible replacement, throw exception
                            throw new KeyNotFoundException(
                                $"The parameter \"{token.Value.ToString()}\" was not present in the lookup dictionary");
                        case MissingKeyBehaviour.ReplaceWithFallback:
                            resultBuilder.Append(fallbackReplacementValue);
                            break;
                        case MissingKeyBehaviour.Ignore:
                            // the replacement value is the input key as a parameter.
                            // use source string and start/length directly with append rather than
                            // parameter.ParameterKey to avoid allocating an extra string
                            resultBuilder.Append(token.Raw);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes a list of format tokens into a string
        /// </summary>
        /// <param name="token">Current token to process</param>
        /// <param name="resultBuilder">The StringBuilder which will contain the result</param>
        /// <param name="replacementParams">Replacement parameters for the FormattableString.</param>
        /// <param name="handler">The function used to perform the replacements on the format tokens</param>
        /// <param name="missingKeyBehaviour">The behaviour to use when the format string contains a parameter that is not present in the lookup dictionary</param>
        /// <param name="fallbackReplacementValue">When the <see cref="MissingKeyBehaviour.ReplaceWithFallback"/> is specified, this string is used as a fallback replacement value when the parameter is present in the lookup dictionary.</param>
        /// <param name="placeholderIndex">The index of the current placeholder in the composite format string</param>
        /// <returns>The processed result of joining the tokens with the replacement dictionary.</returns>
        public static void ProcessTokenIntoFormattableString(
            FormatToken token,
            StringBuilder resultBuilder,
            List<object> replacementParams,
            Func<string, ReplacementResult> handler,
            MissingKeyBehaviour missingKeyBehaviour,
            object fallbackReplacementValue,
            ref int placeholderIndex)
        {
            if (token.TokenType == TokenType.Text)
            {
                // token is a text token.
                // add the token to the result string builder.
                // because this text is going into a standard composite format string,
                // any instaces of { or } must be escaped with {{ and }}
                resultBuilder.AppendWithEscapedBrackets(token.Raw);
            }
            else if (token.TokenType == TokenType.Parameter)
            {
                // token is a parameter token
                // perform parameter logic now.
                // TODO: Look at removing the ToString on tokenKey/format.
                var tokenKey = token.Value.ToString();
                string format = null;
                var separatorIdx = tokenKey.IndexOf(':');
                if (separatorIdx > -1)
                {
                    tokenKey = token.Value.Slice(0, separatorIdx).ToString();
                    format = token.Value.Slice(separatorIdx + 1).ToString();
                }

                // append the replacement for this parameter
                ReplacementResult replacementResult = handler(tokenKey);

                string IndexAndFormat(int index)
                {
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        return "{" + index + "}";
                    }

                    return "{" + index + ":" + format + "}";
                }

                // append the replacement for this parameter
                if (replacementResult.Success)
                {
                    // Instead of appending the replacement value directly as before,
                    // append the next placeholder with the current placeholder index.
                    // Add the actual replacement format item into the replacement values.
                    resultBuilder.Append(IndexAndFormat(placeholderIndex));
                    placeholderIndex++;
                    replacementParams.Add(replacementResult.Value);
                }
                else
                {
                    // the key does not exist, handle this using the missing key behaviour specified.
                    switch (missingKeyBehaviour)
                    {
                        case MissingKeyBehaviour.ThrowException:
                            // the key was not found as a possible replacement, throw exception
                            throw new KeyNotFoundException(
                                $"The parameter \"{token.Value.ToString()}\" was not present in the lookup dictionary");
                        case MissingKeyBehaviour.ReplaceWithFallback:
                            // Instead of appending the replacement value directly as before,
                            // append the next placeholder with the current placeholder index.
                            // Add the actual replacement format item into the replacement values.
                            resultBuilder.Append(IndexAndFormat(placeholderIndex));
                            placeholderIndex++;
                            replacementParams.Add(fallbackReplacementValue);
                            break;
                        case MissingKeyBehaviour.Ignore:
                            resultBuilder.AppendWithEscapedBrackets(token.Raw);
                            break;
                    }
                }
            }
        }
    }
}
