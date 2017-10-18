using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith
{
    public struct ReplacementResult
    {
        public ReplacementResult(bool success, object value)
        {
            Success = success;
            Value = value;
        }

        public bool Success { get; }
        public object Value { get; }
    }
}
