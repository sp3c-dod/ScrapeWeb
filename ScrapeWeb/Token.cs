namespace ScrapeWeb
{
    /// <summary>
    /// The method of how the token will be compared (e.g. starts with, contains, etc...)
    /// </summary>
    public enum TokenType
    {
        RegEx, //https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-6.0
        Equals,
        Contains,
        StartsWith,
        EndsWith
    }

    /// <summary>
    /// Which aspect of the anchor element will be used in the comparison
    /// </summary>
    public enum CompareLocation
    {
        HrefAttribute,
        InnerText
    }

    /// <summary>
    /// Used to determine a match against a given term
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Used to determine a match against a given term with a default CompareLocation of HrefAttribute
        /// </summary>
        /// <param name="type">The method of how the token will be compared (e.g. starts with, contains, etc...)</param>
        /// <param name="pattern">The text that will be used to be match against a given term</param>
        public Token(TokenType type, string pattern)
        {
            Type = type;
            Pattern = pattern;
            CompareLocation = CompareLocation.HrefAttribute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The method of how the token will be compared (e.g. starts with, contains, etc...)</param>
        /// <param name="pattern">The text that will be used to be match against a given term</param>
        /// <param name="compareLocation">Which aspect of the anchor element will be used in the comparison</param>
        public Token(TokenType type, string pattern, CompareLocation compareLocation)
        {
            Type = type;
            Pattern = pattern;
            CompareLocation = compareLocation;
        }

        /// <summary>
        /// The method of how the token will be compared (e.g. starts with, contains, etc...)
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// The text that will be used to be match against a given term
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Which aspect of the anchor element will be used in the comparison
        /// </summary>
        public CompareLocation CompareLocation { get; set; }
    }
}
