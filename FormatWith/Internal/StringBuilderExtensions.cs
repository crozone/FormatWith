using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith.Internal
{
    internal static class StringBuilderExtensions
    {
        public static void AppendWithEscapedBrackets(
                this StringBuilder stringBuilder,
                string value,
                int startIndex,
                int count,
                char openBraceChar = '{',
                char closeBraceChar = '}')
        {
            int currentTokenStart = startIndex;
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (value[i] == openBraceChar)
                {
                    stringBuilder.Append(value, currentTokenStart, i - currentTokenStart);
                    stringBuilder.Append(openBraceChar);
                    currentTokenStart = i;
                }
                else if (value[i] == closeBraceChar)
                {
                    stringBuilder.Append(value, currentTokenStart, i - currentTokenStart);
                    stringBuilder.Append(closeBraceChar);
                    currentTokenStart = i;
                }
            }

            // add the final section
            stringBuilder.Append(value, currentTokenStart, (startIndex + count) - currentTokenStart);
        }
    }
}
