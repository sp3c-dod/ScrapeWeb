using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace ScrapeWeb
{
    /// <summary>
    /// Used to download all map files from https://en.ds-servers.com/maps/goldsrc/dod/
    /// </summary>
    public class WebPaginatedListingDownloader : WebDownloader
    {
        private PaginatedListingSiteInformation _paginatedListingSiteInformation { get; set; }

        /// <summary>
        /// Always download all files on a website
        /// </summary>
        /// <param name="pageMask">URL that represents a page of files. Use {0} in place of where the page number appears.</param>
        /// <param name="pageStart">Start page to start downloading files</param>
        /// <param name="pageEnd">End page to start downloading files</param>
        /// <param name="downloadLinkTransform">Optional RegEx to transform the filename to be downloaded (e.g. .html to .zip)</param>
        /// <param name="serverDownloadInformation">Information about the website whose files will be downloaded</param>
        public WebPaginatedListingDownloader(PaginatedListingSiteInformation serverDownloadInformation) : base(serverDownloadInformation)
        {
            if (String.IsNullOrWhiteSpace(serverDownloadInformation.PageMask))
            {
                throw new ArgumentException("Page Mask is required.");
            }

            if (serverDownloadInformation.PageStart > serverDownloadInformation.PageEnd)
            {
                throw new ArgumentOutOfRangeException("Page Start cannot be greater than Page End");
            }

            _paginatedListingSiteInformation = serverDownloadInformation;
        }

        /// <summary>
        /// Download all links on a directory listing style website.
        /// </summary>
        /// <param name="url">URL containing the files and folders to download</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        /// <returns></returns>
        protected override void DownloadAllLinks(Uri url, string downloadPath)
        {
            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath) && !_serverDownloadInformation.SimulateOnly)
            {
                Directory.CreateDirectory(downloadPath);
            }

            int pageNumber = -1;
            try
            {
                for (pageNumber = _paginatedListingSiteInformation.PageStart; pageNumber <= _paginatedListingSiteInformation.PageEnd; pageNumber++)
                {
                    Console.WriteLine("Downloading page {0}...", pageNumber);
                    DownloadFilesOnPage(String.Format(_paginatedListingSiteInformation.PageMask, pageNumber), downloadPath);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                if (pageNumber > 0)
                {
                    Console.WriteLine("Page " + pageNumber + " does not exit");
                }
            }
        }

        private void DownloadFilesOnPage(string pageUrl, string downloadPath)
        {
            HtmlWeb web = new HtmlWeb();
            Uri url = new Uri(pageUrl);
            HtmlDocument doc = web.Load(url);

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");

            foreach(var anchor in anchors)
            {
                string anchorInnerText = anchor.InnerText;
                string anchorInnerHtml = anchor.InnerHtml;

                // If the anchor tag has no HREF attribute then skip that anchor tag
                HtmlAttribute hrefAttribute = WebUtility.GetHrefAttribute(anchor.Attributes);
                if (hrefAttribute == null)
                {
                    continue;
                }
                string anchorHref = hrefAttribute.Value;
                string decodedAnchorHref = HttpUtility.UrlDecode(anchorHref);

                // Skip any links in the IgnoreTokens collection. This usually includes things such "./" and "../"
                // as well as file types that you don't want to download (e.g. Thumbs.db, *.txt, etc...)
                if (CompareAllTokens(decodedAnchorHref, anchorInnerText, anchorInnerHtml, _serverDownloadInformation.IgnoreTokens))
                {
                    continue;
                }

                WebClient Client = new WebClient();
                try
                {
                    //TODO: download links might not be base URL + relative filename.  Might need to rework this
                    string relativeFileName = anchorHref.Contains("/") ? anchorHref.Remove(0, anchorHref.LastIndexOf("/") + 1) : anchorHref;

                    // Optionally transform the filename (e.g. from .html to .zip)
                    if (_paginatedListingSiteInformation.DownloadLinkTransform != null)
                    {
                        relativeFileName = _paginatedListingSiteInformation.DownloadLinkTransform.Replace(relativeFileName);
                    }

                    Uri downloadLink = new Uri(url, relativeFileName);
                    string decodedFilename = HttpUtility.UrlDecode(relativeFileName);
                    var downloadFilePath = Path.Combine(downloadPath, decodedFilename);

                    if (!_serverDownloadInformation.SimulateOnly)
                    {
                        Console.WriteLine("Downloading file: " + decodedFilename);
                        Client.DownloadFile(downloadLink, downloadFilePath);
                    }

                    _downloadList.Add(downloadFilePath);
                    //_downloadList.Add(decodedAnchorHref);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error downloading: " + pageUrl + anchorHref);
                    Console.WriteLine("    Error Message: " + ex.Message);
                    // Continue downloading the remaining files, so that if there are just a few errors
                    // those files can be manually downloaded.  If there is a large number of errors
                    // the local folder can be deleted and the process retried after the error is resolved.
                }

            }
        }
    }
}
