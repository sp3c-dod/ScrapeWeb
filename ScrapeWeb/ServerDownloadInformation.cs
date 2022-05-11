using System;

namespace ScrapeWeb
{
    public class ServerDownloadInformation
    {
        public Uri ServerUri { get; set; }
        public string DownloadPath { get; set; }
        public bool SimulateOnly { get; set; }
        public string SimulationOutputPath { get; set; }
    }
}
