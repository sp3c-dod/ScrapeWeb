using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace ScrapeWeb
{
    public class WebDownloader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverDownloadInformation"></param>
        public WebDownloader(ServerDownloadInformation serverDownloadInformation)
        {
            _serverDownloadInformation = serverDownloadInformation;
        }

        private  ServerDownloadInformation _serverDownloadInformation { get; set; }
        private List<string> _downloadList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> DownloadAll()
        {
            if (_serverDownloadInformation.ServerUri == null)
            {
                throw new ArgumentException("Server URI is not set");
            }

            if (_serverDownloadInformation.DownloadPath == null)
            {
                throw new ArgumentException("Download Path is not set");
            }

            DownloadAllLinks(_serverDownloadInformation.ServerUri, _serverDownloadInformation.DownloadPath);

            return _downloadList;
        }

        //TODO: links to skip
        //TODO: tokens for folders
        //TODO: file mask tokens
        /// <summary>
        /// Download all links on directory listing style website.
        /// </summary>
        /// <param name="url">Starting URL</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        /// <returns></returns>
        private void DownloadAllLinks(Uri url, string downloadPath)
        {
            //TODO: need downloadListBuilder outside of recursion
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);  //TODO: Why is it loading the wrong thing?

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");

            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath) && !_serverDownloadInformation.SimulateOnly)
            {
                Directory.CreateDirectory(downloadPath);
            }

            foreach (HtmlNode anchor in anchors)
            {
                string anchorHref;
                string anchorInnerText = anchor.InnerText;
                try
                {
                    anchorHref = GetHrefAttribute(anchor.Attributes).Value;
                }
                catch (Exception ex)
                {
                    Exception missingHrefException = new Exception("Could not determine the href attribute of the anchor tag", ex);
                    missingHrefException.Data.Add("InnerHtml", anchor.InnerHtml);
                    missingHrefException.Data.Add("OuterHtml", anchor.OuterHtml);
                    missingHrefException.Data.Add("Url", url.ToString());
                    throw missingHrefException;
                }

                // Skip any links in the IgnoreTokens collection. This usually includes things such "./" and "../"
                // as well as file types that you don't want to download (e.g. Thumbs.db, *.txt, etc...)
                if (CompareAllTokens(anchorHref, anchorInnerText, _serverDownloadInformation.IgnoreTokens))
                {
                    continue;
                }

                //TODO: Check directory tokens
                if (anchorHref.Contains("/"))
                {
                    // Rescurse sub-folder
                    Uri subFolder = new Uri(url, anchorHref);
                    DownloadAllLinks(subFolder, Path.Combine(downloadPath + anchorHref.Replace(@"/", @"\")));
                }
                else
                {
                    WebClient Client = new WebClient();
                    try
                    {
                        Uri downloadLink = new Uri(url, anchorHref);
                        if (!_serverDownloadInformation.SimulateOnly)
                        {
                            Client.DownloadFile(downloadLink, Path.Combine(downloadPath, anchorHref));
                        }

                        _downloadList.Add(Path.Combine(downloadPath, anchorHref));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error downloading: " + url + anchorHref);
                        Console.WriteLine("    Error Message: " + ex.Message);
                    }
                }
            }
        }

        private HtmlAttribute GetHrefAttribute(HtmlAttributeCollection attributes)
        {
            HtmlAttribute hrefAttribute = null;
            foreach(var attribute in attributes)
            {
                if (attribute.Name == "href")
                {
                    hrefAttribute = attribute;
                    break;
                }
            }

            return hrefAttribute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toMatch"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private bool CompareAllTokens(string anchorHref, string anchorInnerText, List<Token> tokens)
        {
            bool matchFound = false;

            foreach (Token token in tokens)
            {
                if (token.CompareLocation == CompareLocation.HrefAttribute)
                {
                    matchFound = MatchToken(anchorHref, token);
                }
                else if (token.CompareLocation == CompareLocation.InnerText)
                {
                    matchFound = MatchToken(anchorInnerText, token);
                }

                // Short circut as soon as a match is found
                if (matchFound) break;
            }

            return matchFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toMatch"></param>
        /// <param name="token"></param>
        /// <remarks>
        /// Yes, I realize all of the token types can be done with regular expressions.
        /// The others were thrown in for ease of use.
        /// </remarks>
        /// <returns></returns>
        private bool MatchToken(string toMatch, Token token)
        {
            bool matched;

            switch (token.Type)
            {
                case TokenType.RegEx:
                    //https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-6.0
                    Regex regEx = new Regex(token.Pattern);
                    matched = regEx.Match(toMatch).Success;
                    break;
                case TokenType.Contains:
                    matched = toMatch.Contains(token.Pattern);
                    break;
                case TokenType.Equals:
                    matched = toMatch.Equals(token.Pattern);
                    break;
                case TokenType.StartsWith:
                    matched = toMatch.StartsWith(token.Pattern);
                    break;
                case TokenType.EndsWith:
                    matched = toMatch.EndsWith(token.Pattern);
                    break;
                default:
                    matched = false;
                    break;
            }

            return matched;
        }
    }
}
