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

        public static FormatToken Create(TokenKind tokenType, ReadOnlySpan<char> token)
        {
            ReadOnlySpan<char> raw = token;
            ReadOnlySpan<char> value = tokenType switch
            {
                // TODO: Make this Trim() configurable
                TokenKind.Parameter => token.Slice(1, token.Length - 2).Trim(),
                TokenKind.Text => token,
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
