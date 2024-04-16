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
using DynamicContentUpdater;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        private const string RecentDataFile = "RecentData.json";
        private const string BlogPostsFile = "BlogPosts.json";
        private const string TagLinksFile = "TagLinks.json";
        private const string ComicsFile = "Comics.json";
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

            var parser = new PostParser(new HardCodedConfig());

            var contentPath = parsedArgs.AppRootPath + "/ProgrammerAl.Site.Content";

            var allPosts = LoadAllPostInfos(contentPath, parser);

            var parsedPostEntries = allPosts
                .OrderBy(x => x.PostDate)//All blog posts start with the date they were posted. Order them so the oldest is first
                .ToImmutableList();

            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            int blogPostNumber = 1;
            var allBlogPostSummaries = parsedPostEntries.Select(x =>
            new PostSummary(
                title: x.Entry.Title,
                postedDate: x.PostDate,
                firstParagraph: Markdig.Markdown.ToHtml(x.Entry.FirstParagraph, pipeline: markdownPipeline),
                postNumber: blogPostNumber++,
                titleLink: x.PostName,
                tags: x.Entry.Tags.ToArray()
            )).ToImmutableArray();

            CreateMetadataJsonFiles(contentPath, allBlogPostSummaries, parsedPostEntries, allPosts);

            //Load the static templating engine
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
            foreach (var postInfo in parsedPostEntries)
            {
                var htmlContent = Markdig.Markdown.ToHtml(postInfo.Entry.Post);
                var htmlEntry = new PostEntry(
                    postInfo.Entry.Title,
                    postInfo.Entry.ReleaseDate,
                    postInfo.Entry.Tags,
                    htmlContent,
                    postInfo.Entry.FirstParagraph);

                string staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", htmlEntry);

                string outputFilePath = $"{outputfolderPath}/{postInfo.PostName}/post.html";
                File.WriteAllText(outputFilePath, staticHtml);
            }

            var sitemapFilePath = parsedArgs.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";
            var sitemapText = GenerateSitemapFile("https://www.programmeral.com/", allPosts);
            File.WriteAllText(sitemapFilePath, sitemapText);
        }

        /// <summary>
        /// Generates the metadata json files that are used by the site to display blog post summaries and tag links
        /// These are loaded by the site to display the most recent blog posts and the tags that are available
        /// </summary>
        private static void CreateMetadataJsonFiles(string contentPath, ImmutableArray<PostSummary> allBlogPostSummaries, ImmutableList<PostInfo> parsedBlogEntries, ImmutableList<PostInfo> allPosts)
        {
            PostSummary[] mostRecentBlogPosts = allBlogPostSummaries
                            .OrderByDescending(x => x.PostNumber)
                            .Take(FrontPageBlogsDisplayed)
                            .ToArray();

            var recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };
            var tagLinks = GenerateTagLinks(allPosts);

            var comicPostSummaries = allBlogPostSummaries
                .Where(x => x.IsComic())
                .Select(x => x.ToComicPostSummary())
                .ToImmutableArray();

            WriteOutFileAsJson(recentData, contentPath, RecentDataFile);
            WriteOutFileAsJson(allBlogPostSummaries, contentPath, BlogPostsFile);
            WriteOutFileAsJson(tagLinks, contentPath, TagLinksFile);
            WriteOutFileAsJson(comicPostSummaries, contentPath, ComicsFile);
        }

        private static TagLinks GenerateTagLinks(ImmutableList<PostInfo> allPosts)
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
                    tagDictionary[tag].Add(post.PostName);
                }
            }

            tagLinks.Links = tagDictionary.ToDictionary(x => x.Key, x => x.Value.ToArray());
            return tagLinks;
        }

        private static string GenerateSitemapFile(string siteUrl, ImmutableList<PostInfo> allPosts)
        {
            var xmlDoc = new XmlDocument();
            var urlSetElement = xmlDoc.CreateElement("urlset", SitemapXmlNamespace);
            _ = xmlDoc.AppendChild(urlSetElement);

            var lastModifiedString = DateTime.Now.ToString("yyyy-MM-dd");
            foreach (var post in allPosts)
            {
                var urlNode = xmlDoc.CreateElement("url", SitemapXmlNamespace);

                var locationNode = xmlDoc.CreateElement("loc", SitemapXmlNamespace);
                locationNode.InnerText = siteUrl + "blog/posts/" + post.PostName;

                var lastModifiedNode = xmlDoc.CreateElement("lastmod", SitemapXmlNamespace);
                lastModifiedNode.InnerText = lastModifiedString;

                _ = urlNode.AppendChild(locationNode);
                _ = urlNode.AppendChild(lastModifiedNode);
                _ = urlSetElement.AppendChild(urlNode);
            }

            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + xmlDoc.InnerXml;
        }

        public static ImmutableList<PostInfo> LoadAllPostInfos(string contentPath, PostParser parser)
        {
            string blogPostsFolderPath = contentPath + "/Posts";
            string[] blogPostFolders = Directory.GetDirectories(blogPostsFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            return blogPostFolders
                .Where(x => !x.StartsWith("draft__", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
            {
                var dirInfo = new DirectoryInfo(x);
                var postFilePath = $"{x}/post.md";
                var postName = dirInfo.Name; // Name of post is the name of the folder

                if (!File.Exists(postFilePath))
                {
                    throw new Exception("$Could not find post file: {postFilePath}");
                }

                var postDateString = postName.Substring(0, 8);
                var postDate = DateOnly.ParseExact(postDateString, "yyyyMMdd");
                string postContent = File.ReadAllText(postFilePath);
                var markdownEntry = parser.ParseFromMarkdown(postContent);
                return new PostInfo(postName, postDate, markdownEntry);
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
