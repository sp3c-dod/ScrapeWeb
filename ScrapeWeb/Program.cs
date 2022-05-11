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
            var serversToDownload = new List<ServerDownloadInformation>()
            {
                new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/198.144.183.130_27016/"), DownloadPath = @"C:\server download\198.144.183.130_27016\", SimulateOnly = true, SimulationOutputPath = "198.144.183.130_27016 simulation.txt" },
                new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/72.251.228.169_27016/"), DownloadPath = @"C:\server download\72.251.228.169_27016\", SimulateOnly = true, SimulationOutputPath = "72.251.228.169_27016 simulation.txt" },
                new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.187.33_27017/"), DownloadPath = @"C:\server download\186.233.187.33_27017\", SimulateOnly = true, SimulationOutputPath = "186.233.187.33_27017 simulation.txt" },
                new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.187.19_27017/"), DownloadPath = @"C:\server download\186.233.187.19_27017\", SimulateOnly = true, SimulationOutputPath = "186.233.187.19_27017 simulation.txt" },
                new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.186.51_27017"), DownloadPath = @"C:\server download\186.233.186.51_27017\", SimulateOnly = true, SimulationOutputPath = "186.233.186.51_27017 simulation.txt" }
            };

            foreach (var serverToDownload in serversToDownload)
            {
                Console.WriteLine("Downloading: " + serverToDownload.ServerUri.ToString());

                if (serverToDownload.SimulateOnly)
                {
                    List<string> simulationDownloadList = new List<string>();
                    DownloadAllLinks(serverToDownload.ServerUri, serverToDownload.DownloadPath, simulationDownloadList);

                    File.WriteAllLines(serverToDownload.SimulationOutputPath, simulationDownloadList.ToArray());
                }
                else
                {
                    DownloadAllLinks(serverToDownload.ServerUri, serverToDownload.DownloadPath);
                }
            }
        }

        //TODO: links to skip
        //TODO: tokens for folders
        //TODO: file mask tokens
        /// <summary>
        /// Download all links on directory listing style website.
        /// </summary>
        /// <param name="url">Starting URL</param>
        /// <param name="downloadPath">Path to store the files locally</param>
        private static void DownloadAllLinks(Uri url, string downloadPath, List<string> simulationDownloadList = null)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Get all the anchors (a elements) on the current page
            IEnumerable<HtmlNode> anchors = doc.DocumentNode.Descendants("a");

            // Create folder if it doesn't exist
            if (!Directory.Exists(downloadPath) && simulationDownloadList == null)
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
                    DownloadAllLinks(subFolder, Path.Combine(downloadPath + anchorInnerText.Replace(@"/", @"\")), simulationDownloadList);
                }
                else
                {
                    WebClient Client = new WebClient();
                    try
                    {
                        Uri downloadLink = new Uri(url, anchorInnerText);
                        if (simulationDownloadList == null)
                        {
                            Client.DownloadFile(downloadLink, Path.Combine(downloadPath, anchorInnerText));
                        }
                        else
                        {
                            simulationDownloadList.Add(Path.Combine(downloadPath, anchorInnerText));
                        }                        
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
