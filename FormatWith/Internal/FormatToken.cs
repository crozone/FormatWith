using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith.Internal
{
    internal ref struct FormatToken
    {
        public FormatToken(TokenType tokenType, ReadOnlySpan<char> raw, ReadOnlySpan<char> value) {
            TokenType = tokenType;
            Raw = raw;
            Value = value;
        }

        /// <summary>
        /// The kind of this token.
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// The complete value of the token, including any enclosing brackets.
        /// </summary>
        public ReadOnlySpan<char> Raw { get; }

        /// <summary>
        /// The token inner value.
        /// </summary>
        public ReadOnlySpan<char> Value { get; }

        public static FormatToken Create(TokenType tokenType, ReadOnlySpan<char> source, int startIndex, int length)
        {
            ReadOnlySpan<char> raw = source.Slice(startIndex, length);
            ReadOnlySpan<char> value = tokenType switch
            {
                TokenType.Parameter => source.Slice(startIndex + 1, length - 2),
                TokenType.Text => source.Slice(startIndex, length),
                _ => throw new InvalidOperationException($"Unknown {nameof(Internal.TokenType)}: {tokenType}"),
            };

            return new FormatToken(tokenType, raw, value);
        }
    }

    internal enum TokenType
    {
        Parameter,
        Text
    }
}
