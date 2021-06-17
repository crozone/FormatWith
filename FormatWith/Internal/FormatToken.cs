using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith.Internal
{
    internal ref struct FormatToken
    {
        public FormatToken(TokenType tokenType, ReadOnlySpan<char> source, int startIndex, int length)
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
        public ReadOnlySpan<char> SourceString { get; }

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
        /// </summary>
        public ReadOnlySpan<char> Raw => SourceString.Slice(StartIndex, Length);

        /// <summary>
        /// Gets the token inner text.
        /// </summary>
        public ReadOnlySpan<char> Value {
            get {
                if (TokenType == TokenType.Parameter)
                {
                    return SourceString.Slice(StartIndex + 1, Length - 2);
                }

                return SourceString.Slice(StartIndex, Length);
            }
        }
    }

    internal enum TokenType
    {
        Parameter,
        Text
    }
}
