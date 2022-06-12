using System;
using System.Collections.Generic;

namespace ScrapeWeb
{
    public abstract class WebDownloader
    {
        /// <summary>
        /// Always download all files on a website
        /// </summary>
        /// <param name="serverDownloadInformation">Information about the website whose files will be downloaded</param>
        public WebDownloader(ServerDownloadInformation serverDownloadInformation)
        {
            _serverDownloadInformation = serverDownloadInformation;
        }

        protected ServerDownloadInformation _serverDownloadInformation { get; set; }
        protected List<string> _downloadList = new List<string>();

        /// <summary>
        /// Downloads all files for a given website
        /// </summary>
        /// <returns>A list of URIs to the files that were downloaded</returns>
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

        /// <summary>
        /// Download all links on a directory listing style website.
        /// </summary>
        /// <param name="url">URL containing the files and folders to download</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        /// <returns></returns>
        abstract protected void DownloadAllLinks(Uri url, string downloadPath);

        /// <summary>
        /// Compare all of the tokens in a given token collection
        /// </summary>
        /// <param name="toMatch">The term to match the token against</param>
        /// <param name="tokens">The tokens to run against the term</param>
        /// <returns>Whether or not any of the tokens match against the given term</returns>
        protected bool CompareAllTokens(string anchorHref, string anchorInnerText, string anchorInnerHtml, List<Token> tokens)
        {
            bool matchFound = false;

            foreach (Token token in tokens)
            {
                if (token.CompareLocation == CompareLocation.HrefAttribute)
                {
                    matchFound = token.Match(anchorHref);
                }
                else if (token.CompareLocation == CompareLocation.InnerHtml)
                {
                    matchFound = token.Match(anchorInnerHtml);
                }
                else if (token.CompareLocation == CompareLocation.InnerText)
                {
                    matchFound = token.Match(anchorInnerText);
                }

                // Short circut as soon as a match is found
                if (matchFound) break;
            }

            return matchFound;
        }
    }
}
