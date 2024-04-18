using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using ProgrammerAl.Site.PostDataEntities;

namespace DynamicContentUpdater.Outputters;

public class SiteMapOutputter
{
    private const string SiteUrl = "https://www.programmeral.com";
    private const string SitemapXmlNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public void Output(string sitemapFilePath, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting sitemap file to {sitemapFilePath}");

        var sitemapFileText = GenerateSitemapFile(allPosts);
        File.WriteAllText(sitemapFilePath, sitemapFileText);

        Console.WriteLine($"Completed output of sitemap file to {sitemapFilePath}");
    }

    private static string GenerateSitemapFile(ImmutableArray<PostEntry> allPosts)
    {
        var xmlDoc = new XmlDocument();
        var urlSetElement = xmlDoc.CreateElement("urlset", SitemapXmlNamespace);
        _ = xmlDoc.AppendChild(urlSetElement);

        var lastModifiedString = DateTime.Now.ToString("yyyy-MM-dd");
        foreach (var post in allPosts)
        {
            var urlNode = xmlDoc.CreateElement("url", SitemapXmlNamespace);

            var locationNode = xmlDoc.CreateElement("loc", SitemapXmlNamespace);
            locationNode.InnerText = $"{SiteUrl}/posts/{post.TitleLink}";

            var lastModifiedNode = xmlDoc.CreateElement("lastmod", SitemapXmlNamespace);
            lastModifiedNode.InnerText = lastModifiedString;

            _ = urlNode.AppendChild(locationNode);
            _ = urlNode.AppendChild(lastModifiedNode);
            _ = urlSetElement.AppendChild(urlNode);
        }

        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + xmlDoc.InnerXml;
    }

}
