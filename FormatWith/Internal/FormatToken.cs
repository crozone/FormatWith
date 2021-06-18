using System;

namespace FormatWith.Internal
{
    internal readonly ref struct FormatToken
    {
        public FormatToken(TokenKind tokenKind, ReadOnlySpan<char> raw, ReadOnlySpan<char> value) {
            TokenKind = tokenKind;
            Raw = raw;
            Value = value;
        }

        /// <summary>
        /// The kind of this token.
        /// </summary>
        public TokenKind TokenKind { get; }

        /// <summary>
        /// The complete value of the token, including any enclosing brackets.
        /// </summary>
        public ReadOnlySpan<char> Raw { get; }

        /// <summary>
        /// The token inner value.
        /// </summary>
        public ReadOnlySpan<char> Value { get; }

        public static FormatToken Create(TokenKind tokenType, ReadOnlySpan<char> source, int startIndex, int length)
        {
            ReadOnlySpan<char> raw = source.Slice(startIndex, length);
            ReadOnlySpan<char> value = tokenType switch
            {
                TokenKind.Parameter => source.Slice(startIndex + 1, length - 2),
                TokenKind.Text => source.Slice(startIndex, length),
                _ => throw new InvalidOperationException($"Unknown {nameof(Internal.TokenKind)}: {tokenType}"),
            };

            return new FormatToken(tokenType, raw, value);
        }
    }

    internal enum TokenKind
    {
        Parameter,
        Text
    }
}
