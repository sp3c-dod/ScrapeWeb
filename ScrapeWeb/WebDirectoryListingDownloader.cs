using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;

namespace ScrapeWeb
{
    public class WebDirectoryListingDownloader : WebDownloader
    {
        /// <summary>
        /// Always download all files on a website while recursively entering links that are determined
        /// to be related sub-folders
        /// </summary>
        /// <param name="serverDownloadInformation">Information about the website whose files will be downloaded</param>
        public WebDirectoryListingDownloader(ServerDownloadInformation serverDownloadInformation) : base(serverDownloadInformation) { }

        /// <summary>
        /// Download all links on a directory listing style website.
        /// Sub-folders will be mirrored to the local download path.
        /// </summary>
        /// <param name="url">URL containing the files and folders to download</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        /// <returns></returns>
        protected override void DownloadAllLinks(Uri url, string downloadPath)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");

            //Enable to output of folders traversed to console:
            //Console.WriteLine("Download path set to: " + downloadPath);

            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath) && !_serverDownloadInformation.SimulateOnly)
            {
                Directory.CreateDirectory(downloadPath);
            }

            foreach (HtmlNode anchor in anchors)
            {
                // Get the HREF attribute and the InnerText of the anchor element
                string anchorHref;
                string anchorInnerText = anchor.InnerText;
                try
                {
                    anchorHref = WebUtility.GetHrefAttribute(anchor.Attributes).Value;
                }
                catch (Exception ex)
                {
                    Exception missingHrefException = new Exception("Could not determine the href attribute of the anchor tag", ex);
                    missingHrefException.Data.Add("InnerHtml", anchor.InnerHtml);
                    missingHrefException.Data.Add("OuterHtml", anchor.OuterHtml);
                    missingHrefException.Data.Add("Url", url.ToString());
                    throw missingHrefException;
                }

                string decodedAnchorHref = HttpUtility.UrlDecode(anchorHref);

                // Skip any links in the IgnoreTokens collection. This usually includes things such "./" and "../"
                // as well as file types that you don't want to download (e.g. Thumbs.db, *.txt, etc...)
                if (CompareAllTokens(decodedAnchorHref, anchorInnerText, _serverDownloadInformation.IgnoreTokens))
                {
                    continue;
                }
                
                // Download files in this folder and recurse into sub-folders
                if (CompareAllTokens(decodedAnchorHref, anchorInnerText, _serverDownloadInformation.DirectoryTokens))
                {
                    // Rescurse sub-folder
                    Uri subFolder = new Uri(url, anchorHref);

                    //TODO: this only works if the folder has a trailing /.  Change to work if the trailing / is missing.
                    string downloadSubFolderName;
                    if (decodedAnchorHref.Count(s => s == '/') > 1)
                    {
                        downloadSubFolderName = decodedAnchorHref.Substring(decodedAnchorHref.Substring(0, decodedAnchorHref.LastIndexOf("/")).LastIndexOf("/") + 1);
                    }
                    else
                    {
                        downloadSubFolderName = decodedAnchorHref;
                    }

                    DownloadAllLinks(subFolder, Path.Combine(downloadPath + downloadSubFolderName.Replace(@"/", @"\")));
                }
                else
                {
                    WebClient Client = new WebClient();
                    try
                    {
                        string relativeFileName = anchorHref.Contains("/") ? anchorHref.Remove(0, anchorHref.LastIndexOf("/") + 1) : anchorHref;
                        Uri downloadLink = new Uri(url, relativeFileName);
                        string decodedFilename = HttpUtility.UrlDecode(relativeFileName);
                        var downloadFilePath = Path.Combine(downloadPath, decodedFilename);

                        if (!_serverDownloadInformation.SimulateOnly)
                        {
                            Client.DownloadFile(downloadLink, downloadFilePath);
                        }

                        _downloadList.Add(downloadFilePath);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error downloading: " + url + anchorHref);
                        Console.WriteLine("    Error Message: " + ex.Message);
                        // Continue downloading the remaining files, so that if there are just a few errors
                        // those files can be manually downloaded.  If there is a large number of errors
                        // the local folder can be deleted and the process retried after the error is resolved.
                    }
                }
            }
        }
    }
}
