using System;
using System.Collections.Generic;

namespace ScrapeWeb
{
    /// <summary>
    /// Information about the website that contains a directory structure of files
    /// </summary>
    public class ServerDownloadInformation
    {
        /// <summary>
        /// Uri to the website that contains a directory structure of files
        /// </summary>
        public Uri ServerUri { get; set; }

        /// <summary>
        /// Local download path to store the downloaded files
        /// </summary>
        public string DownloadPath { get; set; }

        /// <summary>
        /// Links on the website to be ignored (e.g. file types we don't want, folders to skip, unrelated links, etc...)
        /// </summary>
        public List<Token> IgnoreTokens { get; set; }

        /// <summary>
        /// Tokens to determine what kind of links represent a sub-folder
        /// </summary>
        public List<Token> DirectoryTokens { get; set; }

        /// <summary>
        /// Do not actually download the files, but only do a simluation of what would be downloaded
        /// </summary>
        public bool SimulateOnly { get; set; }

        /// <summary>
        /// Local output path for the results of a simulation. This is an optional feature of the simulation
        /// </summary>
        public string SimulationOutputPath { get; set; }
    }
}
