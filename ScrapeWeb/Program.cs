using System;
using System.Collections.Generic;
using System.IO;

namespace ScrapeWeb
{
    public class Program
    {
        /// <summary>
        /// Will match a string with a / in it as long as no . also exists within the string
        /// </summary>
        /// <remarks>
        /// For Windows paths you would need to match a \ to find a folder path.
        /// A period is also valid in the name of a folder, but since we are parsing website links
        /// there isn't always any way to no for certain if you are linked to another directory structure,
        /// web page, or download. You might have to look for other information in the source of the web page
        /// to find another way to determine what is a folder vs a file.
        /// </remarks>
        private const string MatchForwardSlashButNoDotsRegEx = @"^(?!.*\.).*\/.*$";

        public static void Main(string[] args)
        {
            List<Token> globalIgnoreTokens = new List<Token>()
            {
                new Token(TokenType.Equals, "Thumbs.db"),
                new Token(TokenType.StartsWith, "../"),
                new Token(TokenType.Contains, " - Copy"),
                new Token(TokenType.Contains, "Copy of "),
                new Token(TokenType.EndsWith, ".inf"),
                new Token(TokenType.EndsWith, ".gam"),
                new Token(TokenType.EndsWith, ".vdf"),
                new Token(TokenType.EndsWith, ".ini"),
                new Token(TokenType.EndsWith, ".bat"),
                new Token(TokenType.EndsWith, ".css"),
                new Token(TokenType.EndsWith, ".js"),
                new Token(TokenType.EndsWith, ".stats")
            };

            Token defaultDirectoryRegEx = new Token(TokenType.RegEx, MatchForwardSlashButNoDotsRegEx);

            var serversToDownload = new List<ServerDownloadInformation>()
            {
                new ServerDownloadInformation()
                {
                    ServerUri = new Uri("http://wrathofk007.com/hldswokrold/dod/"),
                    DownloadPath = @"C:\server download\wrathofk007.com\",
                    IgnoreTokens = new List<Token>()
                    {
                        new Token(TokenType.StartsWith, "?"),
                        new Token(TokenType.Equals, "Parent Directory", CompareLocation.InnerText),
                        new Token(TokenType.Equals, "addons", CompareLocation.InnerText)
                    },
                    SimulateOnly = true,
                    DownloadListOutputPath = @"c:\temp\wrathofk007.com simulation.txt"
                },
                //new ServerDownloadInformation() { ServerUri = new Uri("http://89.40.216.145/198.144.183.130_27016/"), DownloadPath = @"C:\server download\198.144.183.130_27016\", SimulateOnly = true },
                //new ServerDownloadInformation() { ServerUri = new Uri("http://89.40.216.145/72.251.228.169_27016/"), DownloadPath = @"C:\server download\72.251.228.169_27016\", SimulateOnly = true, DownloadListOutputPath = @"c:\temp\72.251.228.169_27016 simulation.txt" },
                //new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.187.33_27017/"), DownloadPath = @"C:\server download\186.233.187.33_27017\", SimulateOnly = true },
                //new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.187.19_27017/"), DownloadPath = @"C:\server download\186.233.187.19_27017\", SimulateOnly = true, },
                //new ServerDownloadInformation()  { ServerUri = new Uri("http://89.40.216.145/186.233.186.51_27017"), DownloadPath = @"C:\server download\186.233.186.51_27017\", SimulateOnly = true, }
            };

            foreach (var serverToDownload in serversToDownload)
            {
                // Add global ignores to the server information
                if (serverToDownload.IgnoreTokens == null)
                {
                    serverToDownload.IgnoreTokens = new List<Token>();
                }
                serverToDownload.IgnoreTokens.AddRange(globalIgnoreTokens);

                // An empty list will still be honored and not overwritten otherwise use the default directory finding regex
                if (serverToDownload.DirectoryTokens == null)
                {
                    serverToDownload.DirectoryTokens = new List<Token>() { defaultDirectoryRegEx };
                }

                WebDownloader webDownloader = new WebDownloader(serverToDownload);

                if (serverToDownload.SimulateOnly)
                {
                    Console.Write("Simulating ");
                }

                Console.WriteLine("Downloading files from: " + serverToDownload.ServerUri.ToString());
                List<string> downloadList = webDownloader.DownloadAll();

                //Enable to output all files downloaded to console:
                //downloadList.ForEach(d => Console.WriteLine(d));

                if (Directory.Exists(Path.GetDirectoryName(serverToDownload.DownloadListOutputPath)))
                {
                    File.WriteAllLines(serverToDownload.DownloadListOutputPath, downloadList.ToArray());
                }

                // Space out the output between servers we are downloading from
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
