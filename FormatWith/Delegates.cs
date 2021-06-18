using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith
{
    // TODO: XML documentation for everything in here

    public delegate void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, FormatState state, HandlerResultAction resultAction);
    public delegate void HandlerResultAction(FormatState state, bool success, ReadOnlySpan<char> value);
    public delegate void FallbackAction(FormatState state, FallbackResultAction resultAction);
    public delegate void FallbackResultAction(FormatState state, ReadOnlySpan<char> value);
    public delegate void DestinationWriterAction(ReadOnlySpan<char> next);

    public ref struct FormatState
    {
        internal FormatState(
            DestinationWriterAction destinationWriterAction,
            MissingKeyBehaviour missingKeyBehaviour,
            FallbackAction fallbackReplacementAction,
            ReadOnlySpan<char> tokenRaw,
            ReadOnlySpan<char> tokenKey,
            ReadOnlySpan<char> tokenFormat)
        {
            DestinationWriterAction = destinationWriterAction;
            MissingKeyBehaviour = missingKeyBehaviour;
            FallbackReplacementAction = fallbackReplacementAction;
            TokenRaw = tokenRaw;
            TokenKey = tokenKey;
            TokenFormat = tokenFormat;
        }

        internal DestinationWriterAction DestinationWriterAction { get; }
        internal MissingKeyBehaviour MissingKeyBehaviour { get; }
        internal FallbackAction FallbackReplacementAction { get; }
        internal ReadOnlySpan<char> TokenRaw { get; }
        internal ReadOnlySpan<char> TokenKey { get; }
        internal ReadOnlySpan<char> TokenFormat { get; }
    }
}
