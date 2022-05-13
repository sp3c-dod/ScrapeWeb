using System.Text.RegularExpressions;

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

        /// <summary>
        /// Compare the given term agains the given token
        /// </summary>
        /// <param name="toMatch">The term to compare against the given token</param>
        /// <param name="token">The token to compare against the given term</param>
        /// <remarks>
        /// Yes, I realize all of the token types can be done with regular expressions.
        /// The others were thrown in for ease of use.
        /// </remarks>
        /// <returns>Whether or not the token matches against the given term</returns>
        public bool MatchToken(string toMatch)
        {
            bool matched;

            switch (Type)
            {
                case TokenType.RegEx:
                    //Syntax: https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-6.0
                    //Tester: http://regexstorm.net/tester
                    Regex regEx = new Regex(Pattern);
                    matched = regEx.Match(toMatch).Success;
                    break;
                case TokenType.Contains:
                    matched = toMatch.Contains(Pattern);
                    break;
                case TokenType.Equals:
                    matched = toMatch.Equals(Pattern);
                    break;
                case TokenType.StartsWith:
                    matched = toMatch.StartsWith(Pattern);
                    break;
                case TokenType.EndsWith:
                    matched = toMatch.EndsWith(Pattern);
                    break;
                default:
                    matched = false;
                    break;
            }

            return matched;
        }
    }
}
