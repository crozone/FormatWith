using System;
using System.Collections.Generic;

namespace FormatWith.Internal
{
    // We cannot use Action<FormatToken> as the callback type, since FormatToken is a ref struct.
    // Ref structs cannot be used as a generic type parameter.
    // Instead, we need to define our own delegate for the callback.
    internal delegate void SpanAction(FormatToken token);

    internal static class Tokenizer
    {
        /// <summary>
        /// Tokenizes a named format string into a list of text and parameter tokens.
        /// The supplied action is invoked for each token.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="action">The callback to be invoked for each token.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A list of text and parameter tokens representing the input format string</returns>
        public static void Tokenize(ReadOnlySpan<char> formatString, SpanAction action, char openBraceChar = '{', char closeBraceChar = '}')
        {
            if (formatString == null) throw new ArgumentNullException($"{nameof(formatString)} cannot be null.");

            int currentTokenStart = 0;

            // Start the state machine!

            bool insideBraces = false;

            int index = 0;
            while (index < formatString.Length)
            {
                if (!insideBraces)
                {
                    // Currently not inside a pair of braces in the format string (in "text").
                    if (formatString[index] == openBraceChar)
                    {
                        // Check if the brace is escaped
                        if (index < formatString.Length - 1 && formatString[index + 1] == openBraceChar)
                        {
                            // ESCAPED OPEN BRACE

                            // We have hit an escaped open brace.
                            // Yield current normal text, as well as the first brace
                            action(FormatToken.Create(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                            // Skip over braces
                            index += 2;

                            // Set new current token start and current token length
                            currentTokenStart = index;

                            continue;
                        }
                        else
                        {
                            // START OF PARAMETER

                            // Not an escaped brace, set state to inside brace
                            insideBraces = true;

                            // We are leaving standard text and entering into a parameter
                            // add the text traversed so far as a text token
                            if (currentTokenStart < index)
                            {
                                action(FormatToken.Create(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart)));
                            }

                            // Set the start index of the token to the start of this parameter
                            currentTokenStart = index;

                            index++;

                            continue;
                        }
                    }
                    else if (formatString[index] == closeBraceChar)
                    {
                        // Handle case where closing brace is encountered outside braces
                        if (index < formatString.Length - 1 && formatString[index + 1] == closeBraceChar)
                        {
                            // This is an escaped closing brace, this is okay

                            // Add the current normal text, as well as the first brace, to the
                            // List of tokens as a text token.
                            action(FormatToken.Create(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                            // Skip over braces
                            index += 2;

                            // Set new current token start and current token length
                            currentTokenStart = index;

                            continue;
                        }
                        else
                        {
                            // This is an unescaped closing brace outside of braces.
                            // Throw a format exception
                            throw new FormatException($"Unexpected closing brace at position {index}");
                        }
                    }
                    else
                    {
                        // Move onto next character
                        index++;
                        continue;
                    }
                }
                else
                {
                    // Currently inside a pair of braces in the format string (a parameter)
                    if (formatString[index] == openBraceChar)
                    {
                        // Found an opening brace.
                        // Check if the brace is escaped
                        if (index < formatString.Length - 1 && formatString[index + 1] == openBraceChar)
                        {
                            // There are escaped braces within the key.
                            // This is illegal, throw a format exception
                            throw new FormatException($"Illegal escaped opening braces within a parameter at position {index}");
                        }
                        else
                        {
                            // Not an escaped brace, we have an unexpected opening brace within a pair of braces
                            throw new FormatException($"Unexpected opening brace inside a parameter at position {index}");
                        }
                    }
                    else if (formatString[index] == closeBraceChar)
                    {
                        // END OF PARAMETER

                        // Handle case where closing brace is encountered inside braces
                        // Don't attempt to check for escaped braces here, instead always assume the first brace closes the braces,
                        // since we cannot have escaped braces within parameters.

                        // Add the parameter information to the parameter list
                        action(FormatToken.Create(TokenType.Parameter, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                        // Set the state to be outside of any braces
                        insideBraces = false;

                        // Skip over brace
                        index++;

                        // Update current token start
                        currentTokenStart = index;

                        // Move to next state
                        continue;
                    }
                    else
                    {
                        // Character has no special meaning, and it is part of the current parameter.
                        // Move onto next character
                        index++;
                        continue;
                    }
                }
            }

            // After the loop, if all braces were balanced, we should be outside all braces.
            // If we're not, the input string was misformatted.
            if (insideBraces)
            {
                // Throw an exception to indicate the input string was misformatted.
                throw new FormatException($"The format string ended before the parameter was closed. Position {index}");
            }
            else
            {
                // Outside all braces, they were balanced. Add on any remaining text at the end of the format string as "text".
                if (currentTokenStart < index)
                {
                    action(FormatToken.Create(TokenType.Text, formatString, currentTokenStart, index - currentTokenStart));
                }
            }
        }
    }
}