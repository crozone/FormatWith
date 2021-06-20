using System;

namespace FormatWith
{
    // TODO: XML documentation for everything in here

    public delegate bool HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction);
    public delegate void FallbackAction(ResultAction resultAction);
    public delegate void ResultAction(ReadOnlySpan<char> value);
}
