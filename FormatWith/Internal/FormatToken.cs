using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith.Internal
{
    internal struct FormatToken
    {
        public FormatToken(TokenType tokenType, string source, int startIndex, int length)
        {
            TokenType = tokenType;
            SourceString = source;
            StartIndex = startIndex;
            Length = length;
        }

        public TokenType TokenType { get; }

        /// <summary>
        /// The source format string that the token exists within
        /// </summary>
        public string SourceString { get; }

        /// <summary>
        /// The index of the start of the whole token, relative to the start of the source format string.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// The length of the whole token.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the complete value.
        /// This performs a substring operation and allocates a new string object.
        /// </summary>
        public string Raw {
            get {
                return SourceString.Substring(StartIndex, Length);
            }
        }

        /// <summary>
        /// Gets the token inner text.
        /// This performs a substring operation and allocates a new string object.
        /// </summary>
        public string Value {
            get {
                if (TokenType == TokenType.Parameter)
                {
                    return SourceString.Substring(StartIndex + 1, Length - 2);
                }
                else
                {
                    return SourceString.Substring(StartIndex, Length);
                }
            }
        }
    }

    internal enum TokenType
    {
        Parameter,
        Text
    }
}
