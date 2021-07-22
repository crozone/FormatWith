using System;

namespace FormatWith.Internal
{
    // We cannot use Action<FormatToken> as the callback type, since FormatToken is a ref struct.
    // Ref structs cannot be used as a generic type parameter.
    // Instead, we need to define our own delegate for the callback.

    /// <summary>
    /// Delegate type used for the Tokenizer callback.
    /// </summary>
    /// <param name="token"></param>
    internal delegate void ProcessTokenAction(FormatToken token);

    internal static class Tokenizer
    {
        /// <summary>
        /// Tokenizes a named format string into a list of text and parameter tokens.
        /// The supplied action is invoked for each token.
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo}</param>
        /// <param name="processTokenAction">The callback to be invoked for each token.</param>
        /// <param name="openBraceChar">The character used to begin parameters</param>
        /// <param name="closeBraceChar">The character used to end parameters</param>
        /// <returns>A list of text and parameter tokens representing the input format string</returns>
        public static void Tokenize(ReadOnlySpan<char> formatString, ProcessTokenAction processTokenAction, char openBraceChar = '{', char closeBraceChar = '}')
        {
            if (formatString == null) throw new ArgumentNullException($"{nameof(formatString)} cannot be null.");

            int currentTokenStart = 0;

            // Start the state machine!

            bool insideBraces = false;

            int index = 0;
            while (index < formatString.Length)
            {
                // .IndexOfAny(char, char) uses SIMD intrinsics internally for large performance speedups on large data inputs,
                // which is beneficial for large strings.
                // The performance improvement on small strings of unaligned data is small/none.
                int nextBracketOffset = formatString.Slice(index).IndexOfAny(openBraceChar, closeBraceChar);
                if (nextBracketOffset >= 0)
                {
                    index += nextBracketOffset;
                }
                else
                {
                    index = formatString.Length;
                    break;
                }

                char currentCharacter = formatString[index];
                char nextCharacter = index < formatString.Length - 1 ? formatString[index + 1] : '\0';

                if (!insideBraces)
                {
                    // Currently not inside a pair of braces in the format string (in "text").
                    if (currentCharacter == openBraceChar)
                    {
                        // Check if the brace is escaped
                        if (nextCharacter == openBraceChar)
                        {
                            // ESCAPED OPEN BRACE

                            // We have hit an escaped open brace.
                            // Process the current token as text, as well as adding on a single open brace to the end.
                            processTokenAction(FormatToken.Create(TokenKind.Text, formatString.Slice(currentTokenStart, index - currentTokenStart + 1)));

                            // Advance index over braces
                            index += 2;

                            // Update token start
                            currentTokenStart = index;
                        }
                        else
                        {
                            // START OF PARAMETER

                            // Not an escaped brace, set state to inside brace.
                            insideBraces = true;

                            // We are leaving standard text and entering into a parameter.
                            // Process the text traversed so far as a text token.
                            if (currentTokenStart < index)
                            {
                                processTokenAction(FormatToken.Create(TokenKind.Text, formatString.Slice(currentTokenStart, index - currentTokenStart)));
                            }

                            // Update token start
                            currentTokenStart = index;

                            index++;
                        }
                    }
                    else if (currentCharacter == closeBraceChar)
                    {
                        // Handle case where closing brace is encountered outside braces
                        if (nextCharacter == closeBraceChar)
                        {
                            // This is an escaped closing brace, this is okay

                            // Process the current token as text, as well as adding on a single closed brace to the end.
                            processTokenAction(FormatToken.Create(TokenKind.Text, formatString.Slice(currentTokenStart, index - currentTokenStart + 1)));

                            // Advance index over braces
                            index += 2;

                            // Update token start
                            currentTokenStart = index;
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
                        throw new FormatException($"Unexpected character {currentCharacter}, expected open brace \'{openBraceChar}\' or closed brace \'{closeBraceChar}\'.");
                    }
                }
                else
                {
                    // Currently inside a pair of braces in the format string (a parameter)
                    if (currentCharacter == openBraceChar)
                    {
                        // Found an opening brace.
                        // Check if the brace is escaped
                        if (nextCharacter == openBraceChar)
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
                    else if (currentCharacter == closeBraceChar)
                    {
                        // END OF PARAMETER

                        // Handle case where closing brace is encountered inside braces
                        // Don't attempt to check for escaped braces here, instead always assume the first brace closes the braces,
                        // since we cannot have escaped braces within parameters.

                        // Process the current token as a parameter, which included the opening and closing bracket.
                        processTokenAction(FormatToken.Create(TokenKind.Parameter, formatString.Slice(currentTokenStart, index - currentTokenStart + 1)));

                        // Set the state to be outside of any braces
                        insideBraces = false;

                        // Advance index over brace
                        index++;

                        // Update current token start
                        currentTokenStart = index;
                    }
                    else
                    {
                        throw new FormatException($"Unexpected character {currentCharacter}, expected open brace \'{openBraceChar}\' or closed brace \'{closeBraceChar}\'.");
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
                // Outside all braces, and they were balanced. Process any remaining text at the end of the format string as a text token.
                if (currentTokenStart < index)
                {
                    processTokenAction(FormatToken.Create(TokenKind.Text, formatString.Slice(currentTokenStart, index - currentTokenStart)));
                }
            }
        }
    }
}