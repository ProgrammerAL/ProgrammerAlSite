using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using Newtonsoft.Json;
using System;
using RazorLight;
using System.Threading.Tasks;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Markdig;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        private const string RecentDataFile = "RecentData.json";
        private const string BlogPostsFile = "BlogPosts.json";
        private const string TagLinksFile = "TagLinks.json";
        private const int FrontPageBlogsDisplayed = 5;
        private const string SitemapXmlNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        static async Task Main(string[] args)
        {
            Options parsedArgs = null;

            _ = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => parsedArgs = opts)
                .WithNotParsed((errs) =>
                {
                    Console.WriteLine(errs);
                    return;
                });

            HardCodedConfig config = new HardCodedConfig();
            BlogPostParser parser = new BlogPostParser(config);

            var contentPath = parsedArgs.AppRootPath + "/ProgrammerAl.Site.Content";

            ImmutableList<BlogPostInfo> allPosts = LoadAllBlogPostInfo(contentPath, parser);

            ImmutableList<BlogPostInfo> parsedBlogEntries = allPosts
                .OrderBy(x => x.PostDate)//All blog posts start with the date they were posted. Order them so the oldest is first
                .ToImmutableList();

            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            int blogPostNumber = 1;
            BlogPostSummary[] allBlogPostSummaries = parsedBlogEntries.Select(x => new BlogPostSummary
            {
                Title = x.Entry.Title,
                PostedDate = x.PostDate,
                FirstParagraph = Markdig.Markdown.ToHtml(x.Entry.FirstParagraph, pipeline: markdownPipeline),
                PostNumber = blogPostNumber++,
                TitleLink = x.FileNameWithoutExtension,
                Tags = x.Entry.Tags.ToArray()
            }).ToArray();

            BlogPostSummary[] mostRecentBlogPosts = allBlogPostSummaries
                                        .OrderByDescending(x => x.PostNumber)
                                        .Take(FrontPageBlogsDisplayed)
                                        .ToArray();

            RecentData recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };
            TagLinks tagLinks = GenerateTagLinks(allPosts);

            WriteOutFileAsJson(recentData, contentPath, RecentDataFile);
            WriteOutFileAsJson(allBlogPostSummaries, contentPath, BlogPostsFile);
            WriteOutFileAsJson(tagLinks, contentPath, TagLinksFile);

            //Load up the static templating engine
            var fullPathToTemplates = parsedArgs.AppRootPath + "/ProgrammerAl.Site/DynamicContentUpdater/StaticTemplates";
            var engine = new RazorLightEngineBuilder()
              .UseFileSystemProject(fullPathToTemplates)
              .UseMemoryCachingProvider()
              .Build();

            string outputfolderPath = Path.Combine(contentPath, "BlogPosts");
            if (!Directory.Exists(outputfolderPath))
            {
                _ = Directory.CreateDirectory(outputfolderPath);
            }

            //Create static html files for each blog post entry
            foreach (BlogPostInfo blogEntry in parsedBlogEntries)
            {
                var htmlContent = Markdig.Markdown.ToHtml(blogEntry.Entry.Post);
                var blogPostEntryWithHtml = new PostEntry(blogEntry.Entry.Title, blogEntry.Entry.ReleaseDate, blogEntry.Entry.Tags, htmlContent, blogEntry.Entry.FirstParagraph);

                string staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", blogPostEntryWithHtml);

                string outputFilePath = Path.Combine(outputfolderPath, blogEntry.FileNameWithoutExtension) + ".html";
                File.WriteAllText(outputFilePath, staticHtml);
            }

            var sitemapFilePath = parsedArgs.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";
            var sitemapText = GenerateSitemapFile("https://www.programmeral.com/", allPosts);
            File.WriteAllText(sitemapFilePath, sitemapText);
        }

        private static TagLinks GenerateTagLinks(ImmutableList<BlogPostInfo> allPosts)
        {
            var tagLinks = new TagLinks();
            var tagDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var post in allPosts)
            {
                foreach (var tag in post.Entry.Tags)
                {
                    if (!tagDictionary.ContainsKey(tag))
                    {
                        tagDictionary.Add(tag, new List<string>());
                    }
                    tagDictionary[tag].Add(post.FileNameWithoutExtension);
                }
            }

            tagLinks.Links = tagDictionary.ToDictionary(x => x.Key, x => x.Value.ToArray());
            return tagLinks;
        }

        private static string GenerateSitemapFile(string siteUrl, ImmutableList<BlogPostInfo> allPosts)
        {
            var xmlDoc = new XmlDocument();
            var urlSetElement = xmlDoc.CreateElement("urlset", SitemapXmlNamespace);
            _ = xmlDoc.AppendChild(urlSetElement);

            var lastModifiedString = DateTime.Now.ToString("yyyy-MM-dd");
            foreach (var post in allPosts)
            {
                var urlNode = xmlDoc.CreateElement("url", SitemapXmlNamespace);

                var locationNode = xmlDoc.CreateElement("loc", SitemapXmlNamespace);
                locationNode.InnerText = siteUrl + "blog/posts/" + post.FileNameWithoutExtension;

                var lastModifiedNode = xmlDoc.CreateElement("lastmod", SitemapXmlNamespace);
                lastModifiedNode.InnerText = lastModifiedString;

                _ = urlNode.AppendChild(locationNode);
                _ = urlNode.AppendChild(lastModifiedNode);
                _ = urlSetElement.AppendChild(urlNode);
            }

            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + xmlDoc.InnerXml;
        }

        public static ImmutableList<BlogPostInfo> LoadAllBlogPostInfo(string contentPath, BlogPostParser parser)
        {
            string blogPostsFolderPath = contentPath + "/BlogPosts";
            string[] blogPostFiles = Directory.GetFiles(blogPostsFolderPath, "*.md", SearchOption.TopDirectoryOnly);
            return blogPostFiles.Select(x =>
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(x);
                    var postDateString = fileNameWithoutExtension.Substring(0, 8);
                    var postDate = DateOnly.ParseExact(postDateString, "yyyyMMdd");
                    string postContent = File.ReadAllText(x);
                    PostEntry blogEntry = parser.ParseFromMarkdown(postContent);
                    return new BlogPostInfo(fileNameWithoutExtension, postDate, blogEntry);
                })
                .ToImmutableList();
        }

        private static void WriteOutFileAsJson<T>(T obj, string contentPath, string fileName)
        {
            string objJson = JsonConvert.SerializeObject(obj);
            string outputFilePath = Path.Combine(contentPath, fileName);
            File.WriteAllText(outputFilePath, objJson);
        }
    }
}
