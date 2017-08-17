namespace FormatWith.Internal {
    /// <summary>
    /// Represents a section of non-parameter text within a parent string.
    /// </summary>
    public class TextToken : FormatToken {
        public TextToken(string source, int startIndex, int length) : base(source, startIndex, length) { }
    }
}
