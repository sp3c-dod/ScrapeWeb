using System;
using System.Collections.Generic;

namespace ScrapeWeb
{
    public class ServerDownloadInformation
    {
        public Uri ServerUri { get; set; }
        public string DownloadPath { get; set; }
        public List<Token> IgnoreTokens { get; set; }
        public List<Token> DirectoryTokens { get; set; }
        public bool SimulateOnly { get; set; }
        public string SimulationOutputPath { get; set; }
    }
}
