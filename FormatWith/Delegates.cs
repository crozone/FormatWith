using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith
{
    // TODO: XML documentation for everything in here

    public delegate void HandlerAction(ReadOnlySpan<char> key, ReadOnlySpan<char> format, ResultAction resultAction);
    public delegate void FallbackAction(ResultAction resultAction);
    public delegate void ResultAction(ReadOnlySpan<char> value);
}
