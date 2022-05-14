using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScrapeWeb
{
    /// <summary>
    /// Used to download all map files from https://en.ds-servers.com/maps/goldsrc/dod/
    /// </summary>
    public class WebPaginatedListingDownloader : WebDownloader
    {
        /// <summary>
        /// Determines what kind of link is used to the pages of download files
        /// </summary>
        Token _pageToken;

        /// <summary>
        /// Optional transform on the download link such replacing .html with .zip
        /// </summary>
        Regex _downloadLinkTransform;

        /// <summary>
        /// Always download all files on a website
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="downloadLinkTransform"></param>
        /// <param name="serverDownloadInformation">Information about the website whose files will be downloaded</param>
        public WebPaginatedListingDownloader(ServerDownloadInformation serverDownloadInformation, Token pageToken, Regex downloadLinkTransform) : base(serverDownloadInformation)
        {
            if (pageToken == null)
            {
                throw new ArgumentException("Page Token is required.");
            }

            _pageToken = pageToken;
        }

        /// <summary>
        /// Download all links on a directory listing style website.
        /// </summary>
        /// <param name="url">URL containing the files and folders to download</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        /// <returns></returns>
        protected override void DownloadAllLinks(Uri url, string downloadPath)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath) && !_serverDownloadInformation.SimulateOnly)
            {
                Directory.CreateDirectory(downloadPath);
            }

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");
            List<Uri> pageUris = new List<Uri>();
            List<Uri> downloadUris = new List<Uri>();

            //TODO: what if there a ellipses. find a way to iterate to last page
            // Gather all page links 
            foreach (var anchor in anchors)
            {
                try
                {
                    string anchorHref = WebUtility.GetHrefAttribute(anchor.Attributes).Value;
                    if (_pageToken.Match(anchorHref))
                    {
                        //TODO: what if a relative URL vs an aboslute URL. Account for both here
                        pageUris.Add(new Uri(anchorHref));
                    }
                }
                catch (Exception ex)
                {
                    Exception missingHrefException = new Exception("Could not determine the href attribute of the anchor tag", ex);
                    missingHrefException.Data.Add("InnerHtml", anchor.InnerHtml);
                    missingHrefException.Data.Add("OuterHtml", anchor.OuterHtml);
                    missingHrefException.Data.Add("Url", url.ToString());
                    throw missingHrefException;
                }
            }

            //TODO: start on current page (add to collection) and the iterate for each page
            //TODO: append page links to the ignore lists before downloading

            //if (CompareAllTokens(anchorHref, anchor.InnerText, _serverDownloadInformation.IgnoreTokens))
            //{
            //    //TODO: what if a relative URL vs an aboslute URL. Account for both here
            //    downloadUris.Add(new Uri(anchorHref));
            //}


            //TODO: Do an optional transform on the download link before starting download
            if (_downloadLinkTransform != null)
            {

            }

            //TODO: Download files
            //WebClient Client = new WebClient();
            //try
            //{
            //    string relativeFileName = anchorHref.Contains("/") ? anchorHref.Remove(0, anchorHref.LastIndexOf("/") + 1) : anchorHref;
            //    Uri downloadLink = new Uri(url, relativeFileName);
            //    string decodedFilename = HttpUtility.UrlDecode(relativeFileName);
            //    var downloadFilePath = Path.Combine(downloadPath, decodedFilename);

            //    if (!_serverDownloadInformation.SimulateOnly)
            //    {
            //        Client.DownloadFile(downloadLink, downloadFilePath);
            //    }

            //    _downloadList.Add(downloadFilePath);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error downloading: " + url + anchorHref);
            //    Console.WriteLine("    Error Message: " + ex.Message);
            //    // Continue downloading the remaining files, so that if there are just a few errors
            //    // those files can be manually downloaded.  If there is a large number of errors
            //    // the local folder can be deleted and the process retried after the error is resolved.
            //}
        }
    }
}
