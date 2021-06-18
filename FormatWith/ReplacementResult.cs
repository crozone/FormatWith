namespace FormatWith
{
    /// <summary>
    /// Represents the result of a substitution for a parameter within a format string.
    /// </summary>
    public struct ReplacementResult<TValue>
    {
        /// <summary>
        /// Represents the result of a substitution for a parameter within a format string.
        /// </summary>
        /// <param name="success">Represents whether or not the substitution was successful.</param>
        /// <param name="value">The new value for the substituted format parameter.</param>
        public ReplacementResult(bool success, TValue value)
        {
            Success = success;
            Value = value;
        }

        /// <summary>
        /// Represents whether or not the substitution was successful.
        /// If true, the handler was successfully able to replace this parameter with the substituted value.
        /// If false, the substitution failed, and Value will be set to null.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// The new value for the substituted format parameter.
        /// If Success is false, this should be set to null.
        /// </summary>
        public TValue Value { get; }
    }
}
