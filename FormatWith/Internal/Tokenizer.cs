using System;
using System.Collections.Generic;

namespace FormatWith.Internal
{
    // Can't use Action<FormatToken> as FormatToken is a ref struct
    // which can't be used as a generic type parameter. 
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

            // start the state machine!

            bool insideBraces = false;

            int index = 0;
            while (index < formatString.Length)
            {
                if (!insideBraces)
                {
                    // currently not inside a pair of braces in the format string
                    if (formatString[index] == openBraceChar)
                    {
                        // check if the brace is escaped
                        if (index < formatString.Length - 1 && formatString[index + 1] == openBraceChar)
                        {
                            // ESCAPED OPEN BRACE

                            // we have hit an escaped open brace
                            // return current normal text, as well as the first brace
                            // implemented as yield return, this generates a IEnumerator state machine.
                            action(new FormatToken(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                            // skip over braces
                            index += 2;

                            // set new current token start and current token length
                            currentTokenStart = index;

                            continue;
                        }
                        else
                        {
                            // START OF PARAMETER

                            // not an escaped brace, set state to inside brace
                            insideBraces = true;

                            // we are leaving standard text and entering into a parameter
                            // add the text traversed so far as a text token
                            if (currentTokenStart < index)
                            {
                                action(new FormatToken(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart)));
                            }

                            // set the start index of the token to the start of this parameter
                            currentTokenStart = index;

                            index++;

                            continue;
                        }
                    }
                    else if (formatString[index] == closeBraceChar)
                    {
                        // handle case where closing brace is encountered outside braces
                        if (index < formatString.Length - 1 && formatString[index + 1] == closeBraceChar)
                        {
                            // this is an escaped closing brace, this is okay

                            // add the current normal text, as well as the first brace, to the
                            // list of tokens as a text token.
                            action(new FormatToken(TokenType.Text, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                            // skip over braces
                            index += 2;

                            // set new current token start and current token length
                            currentTokenStart = index;

                            continue;
                        }
                        else
                        {
                            // this is an unescaped closing brace outside of braces.
                            // throw a format exception
                            throw new FormatException($"Unexpected closing brace at position {index}");
                        }
                    }
                    else
                    {
                        // move onto next character
                        index++;
                        continue;
                    }
                }
                else
                {
                    // currently inside a pair of braces in the format string
                    if (formatString[index] == openBraceChar)
                    {
                        // found an opening brace
                        // check if the brace is escaped
                        if (index < formatString.Length - 1 && formatString[index + 1] == openBraceChar)
                        {
                            // there are escaped braces within the key
                            // this is illegal, throw a format exception
                            throw new FormatException($"Illegal escaped opening braces within a parameter at position {index}");
                        }
                        else
                        {
                            // not an escaped brace, we have an unexpected opening brace within a pair of braces
                            throw new FormatException($"Unexpected opening brace inside a parameter at position {index}");
                        }
                    }
                    else if (formatString[index] == closeBraceChar)
                    {
                        // END OF PARAMETER
                        // handle case where closing brace is encountered inside braces
                        // don't attempt to check for escaped braces here - always assume the first brace closes the braces
                        // since we cannot have escaped braces within parameters.

                        // Add the parameter information to the parameter list
                        action(new FormatToken(TokenType.Parameter, formatString, currentTokenStart, (index - currentTokenStart) + 1));

                        // set the state to be outside of any braces
                        insideBraces = false;

                        // jump over brace
                        index++;

                        // update current token start
                        currentTokenStart = index;

                        // jump to next state
                        continue;
                    } // if }
                    else
                    {
                        // character has no special meaning, it is part of the current key
                        // move onto next character
                        index++;
                        continue;
                    } // else
                } // if inside brace
            } // while index < formatString.Length

            // after the loop, if all braces were balanced, we should be outside all braces
            // if we're not, the input string was misformatted.
            if (insideBraces)
            {
                throw new FormatException($"The format string ended before the parameter was closed. Position {index}");
            }
            else
            {
                // outside braces. Add on any remaining text at the end of the format string
                if (currentTokenStart < index)
                {
                    action(new FormatToken(TokenType.Text, formatString, currentTokenStart, index - currentTokenStart));
                }
            }
        }
    }
}