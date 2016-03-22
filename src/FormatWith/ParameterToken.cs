namespace FormatWith {
    public class ParameterToken : FormatToken {
        public ParameterToken(string source, int startIndex, int length) : base(source, startIndex, length) {
            KeyStartIndex = startIndex + 1;
            KeyLength = length - 2;
        }

        /// <summary>
        /// The index of the start of the parameter key
        /// </summary>
        public int KeyStartIndex { get; }


        /// <summary>
        /// The length of the parameter key (excluding surrounding braces)
        /// </summary>
        public int KeyLength { get; }

        private string parameterKey = null;

        /// <summary>
        /// Gets the parameter key (without the surrounding braces), as a string. Note that this performs a substring operation and allocates a new string object. The string object is cached for all subsiquent requests.
        /// </summary>
        public string ParameterKey
        {
            get
            {
                if (parameterKey == null) {
                    parameterKey = SourceString.Substring(KeyStartIndex, KeyLength);
                }

                return parameterKey;
            }
        }
    }
}
