using System;
using System.Text.RegularExpressions;

namespace ScrapeWeb
{
    public class TermReplacer
    {
        private string _termToReplace;
        private Regex _termToReplaceRegEx = null;
        private string _replacementTerm;

        /// <summary>
        /// Allows replacement of one term with another in a given string
        /// </summary>
        /// <param name="termToReplace">Term to replace</param>
        /// <param name="replacementTerm">Replacement Term</param>
        public TermReplacer(string termToReplace, string replacementTerm)
        {
            if (String.IsNullOrEmpty(termToReplace))
            {
                throw new ArgumentException("Term to replace is required");
            }

            // Allow replacing a term with an empty string
            if (replacementTerm == null)
            {
                throw new ArgumentNullException("Replacement term is required");
            }

            _termToReplace = termToReplace;
            _replacementTerm = replacementTerm;
        }

        /// <summary>
        /// Allows replacement of all terms matching a pattern with another in a given string
        /// </summary>
        /// <param name="termToReplace">Regex containing the pattern of terms to be replaced</param>
        /// <param name="replacementTerm">Replacement Term</param>
        public TermReplacer(Regex termToReplace, string replacementTerm)
        {
            if (termToReplace == null)
            {
                throw new ArgumentException("Term to replace is required");
            }

            // Allow replacing a term with an empty string
            if (replacementTerm == null)
            {
                throw new ArgumentNullException("Replacement term is required");
            }


            _termToReplaceRegEx = termToReplace;
            _replacementTerm = replacementTerm;
        }

        /// <summary>
        /// Replace one term (or a pattern of terms) with another term in a given search string
        /// </summary>
        /// <param name="searchString">String to search for the term to replaced</param>
        /// <returns>The string with the term (or pattern of terms) replaced with the replacement term</returns>
        public string Replace(string searchString)
        {
            if (_termToReplaceRegEx != null)
            {
                return _termToReplaceRegEx.Replace(searchString, _replacementTerm);
            }
            else
            {
                return searchString.Replace(_termToReplace, _replacementTerm);
            }
        }
    }
}
