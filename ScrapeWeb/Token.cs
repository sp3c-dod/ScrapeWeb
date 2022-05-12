namespace ScrapeWeb
{
    public enum TokenType
    {
        RegEx, //https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-6.0
        Equals,
        Contains,
        StartsWith,
        EndsWith
    }

    public enum CompareLocation
    {
        HrefAttribute,
        InnerText
    }

    public class Token
    {
        public Token(TokenType type, string pattern)
        {
            Type = type;
            Pattern = pattern;
            CompareLocation = CompareLocation.HrefAttribute;
        }

        public Token(TokenType type, string pattern, CompareLocation compareLocation)
        {
            Type = type;
            Pattern = pattern;
            CompareLocation = compareLocation;
        }

        public TokenType Type { get; set; }
        public string Pattern { get; set; }
        public CompareLocation CompareLocation { get; set; }
    }
}
