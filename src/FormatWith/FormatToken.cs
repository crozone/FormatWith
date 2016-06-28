namespace FormatWith.Internal {
    public abstract class FormatToken {
        public FormatToken(string source, int startIndex, int length) {
            SourceString = source;
            StartIndex = startIndex;
            Length = length;
        }

        /// <summary>
        /// The source format string that the token exists within
        /// </summary>
        public string SourceString { get; }

        /// <summary>
        /// The index of the start of the whole token, relative to the start of the source format string.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// The length of the whole token.
        /// </summary>
        public int Length { get; }

        private string text = null;

        /// <summary>
        /// Gets the complete token text. Note that this performs a substring operation and allocates a new string object. The string object is cached for all subsiquent requests.
        /// </summary>
        public string Text
        {
            get
            {
                if (text == null) {
                    text = SourceString.Substring(StartIndex, Length);
                }

                return text;
            }
        }
    }
}
