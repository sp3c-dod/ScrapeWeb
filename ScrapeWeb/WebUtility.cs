using HtmlAgilityPack;

namespace ScrapeWeb
{
    public static class WebUtility
    {
        public static HtmlAttribute GetHrefAttribute(HtmlAttributeCollection attributes)
        {
            HtmlAttribute hrefAttribute = null;
            foreach (var attribute in attributes)
            {
                if (attribute.Name.ToLower() == "href")
                {
                    hrefAttribute = attribute;
                    break;
                }
            }

            return hrefAttribute;
        }
    }
}
