namespace ScrapeWeb
{
    /// <summary>
    /// Contains information for downloading files from a website with a paginated list of downloads
    /// </summary>
    public class PaginatedListingSiteInformation : ServerDownloadInformation
    {
        /// <summary>
        /// URL that represents a page of files. Use {0} in place of where the page number appears.
        /// </summary>
        public string PageMask { get; set; }

        /// <summary>
        /// Optional transform on the download link such replacing .html with .zip
        /// </summary>
        public TermReplacer DownloadLinkTransform { get; set; }

        /// <summary>
        /// Start page to start downloading files
        /// </summary>
        public int PageStart { get; set; }

        /// <summary>
        /// End page to start downloading files
        /// </summary>
        public int PageEnd { get; set; }
    }
}
