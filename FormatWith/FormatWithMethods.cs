using FormatWith.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith
{
    public static class FormatWithMethods
    {
        public static string FormatWith(
            string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            if (formatString.Length == 0) return string.Empty;
            // get the parameters from the format string
            IEnumerable<FormatToken> tokens = FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar);
            return FormatHelpers.ProcessTokens(tokens, replacements, missingKeyBehaviour, fallbackReplacementValue, formatString.Length * 2);
        }

        public static FormattableString FormattableWith(
            string formatString,
            IDictionary<string, object> replacements,
            MissingKeyBehaviour missingKeyBehaviour = MissingKeyBehaviour.ThrowException,
            object fallbackReplacementValue = null,
            char openBraceChar = '{',
            char closeBraceChar = '}')
        {
            // get the parameters from the format string
            IEnumerable<FormatToken> tokens = FormatHelpers.Tokenize(formatString, openBraceChar, closeBraceChar);
            return FormatHelpers.ProcessTokensIntoFormattableString(tokens, replacements, missingKeyBehaviour, fallbackReplacementValue, formatString.Length * 2);
        }
    }
}
