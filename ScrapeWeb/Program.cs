using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ScrapeWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Uri uri = new Uri("http://89.40.216.145/198.144.183.130_27016/");
            const string downloadPath = @"C:\server download\198.144.183.130_27016\";

            Uri uri1 = new Uri("http://89.40.216.145/72.251.228.169_27016/");
            const string downloadPath1 = @"C:\server download\72.251.228.169_27016\";

            Uri uri2 = new Uri("http://89.40.216.145/186.233.187.33_27017/");
            const string downloadPath2 = @"C:\server download\186.233.187.33_27017\";

            Uri uri3 = new Uri("http://89.40.216.145/186.233.187.19_27017/");
            const string downloadPath3 = @"C:\server download\186.233.187.19_27017\";

            Uri uri4 = new Uri("http://89.40.216.145/186.233.186.51_27017");
            const string downloadPath4 = @"C:\server download\186.233.186.51_27017\";

            Console.WriteLine(uri.ToString());
            DownloadAllLinks(uri, downloadPath);
            Console.WriteLine(uri1.ToString());
            DownloadAllLinks(uri1, downloadPath1);
            Console.WriteLine(uri2.ToString());
            DownloadAllLinks(uri2, downloadPath2);
            Console.WriteLine(uri3.ToString());
            DownloadAllLinks(uri3, downloadPath3);
            Console.WriteLine(uri4.ToString());
            DownloadAllLinks(uri4, downloadPath4);
        }

        /// <summary>
        /// Download all links on directory listing style website.
        /// </summary>
        /// <param name="url">Starting URL</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        private static void DownloadAllLinks(Uri url, string downloadPath)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");

            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            foreach(HtmlNode anchor in anchors)
            {
                string anchorInnerText;
                try
                {
                    anchorInnerText = anchor.Attributes[0].Value;
                }
                catch
                {
                    Console.WriteLine("Error getting anchor link in " + url.ToString());
                    Console.WriteLine(anchor.InnerHtml);
                    Console.WriteLine(anchor.OuterHtml);
                    anchorInnerText = anchor.InnerHtml;
                }

                // Skip the up a directory command link
                if (anchorInnerText.Equals("../"))
                {
                    continue;
                }

                if (anchorInnerText.Contains("/"))
                {
                    // Rescurse sub-folder
                    Uri subFolder = new Uri(url, anchorInnerText);
                    DownloadAllLinks(subFolder, Path.Combine(downloadPath + anchorInnerText.Replace(@"/", @"\")));
                }
                else
                {
                    WebClient Client = new WebClient();
                    try
                    {
                        Uri downloadLink = new Uri(url, anchorInnerText);
                        Client.DownloadFile(downloadLink, Path.Combine(downloadPath, anchorInnerText));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error downloading: " + url + anchorInnerText);
                        Console.WriteLine("    Error Message: " + ex.Message);
                    }
                    
                }
            }
        }
    }
}
