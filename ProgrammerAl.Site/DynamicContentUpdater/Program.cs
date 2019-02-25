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

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        private const string RecentDataFile = "RecentData.json";
        private const string BlogPostsFile = "BlogPosts.json";
        private const int FrontPageBlogsDisplayed = 5;

        static async Task Main(string[] args)
        {
            Options parsedArgs = null;

            Parser.Default.ParseArguments<Options>(args)
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
                .OrderBy(x => x.FileNameWithoutExtension)//All blog posts start with a number. So the higher the number, the newer the post
                .ToImmutableList();

            int blogPostNumber = 1;
            BlogPostSummary[] allBlogPostSummaries = parsedBlogEntries.Select(x => new BlogPostSummary
            {
                Title = x.Entry.Title,
                PostedDate = x.Entry.ReleaseDate,
                FirstParagraph = x.Entry.FirstParagraph,
                PostNumber = blogPostNumber++,
                TitleLink = x.FileNameWithoutExtension,
            }).ToArray();

            BlogPostSummary[] mostRecentBlogPosts = allBlogPostSummaries
                                        .OrderByDescending(x => x.PostNumber)
                                        .Take(FrontPageBlogsDisplayed)
                                        .ToArray();

            RecentData recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };

            WriteOutFileAsJson(recentData, contentPath, "RecentData.json");
            WriteOutFileAsJson(allBlogPostSummaries, contentPath, "BlogPosts.json");

            //Load up the static templating engine
            var fullPathToTemplates = Path.Combine(Environment.CurrentDirectory, "StaticTemplates");
            var engine = new RazorLightEngineBuilder()
              .UseFilesystemProject(fullPathToTemplates)
              .UseMemoryCachingProvider()
              .Build();

            string outputfolderPath = Path.Combine(contentPath, "BlogPosts");
            if (!Directory.Exists(outputfolderPath))
            {
                Directory.CreateDirectory(outputfolderPath);
            }

            //Create static html files for each blog post entry
            foreach (BlogPostInfo blogEntry in parsedBlogEntries)
            {
                var htmlContent = Markdig.Markdown.ToHtml(blogEntry.Entry.Post);
                var blogPostEntryWithHtml = new BlogPostEntry(blogEntry.Entry.Title, blogEntry.Entry.ReleaseDate, blogEntry.Entry.Tags, htmlContent, blogEntry.Entry.FirstParagraph);

                string staticHtml = await engine.CompileRenderAsync<BlogPostEntry>("BlogPost.cshtml", blogPostEntryWithHtml);

                string outputFilePath = Path.Combine(outputfolderPath, blogEntry.FileNameWithoutExtension) + ".html";
                File.WriteAllText(outputFilePath, staticHtml);
            }

            var sitemapFilePath = parsedArgs.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";
            var sitemapText = GenerateSitemapFile("https://www.programmeral.com/", allPosts);
            File.WriteAllText(sitemapFilePath, sitemapText);
        }

        private static string GenerateSitemapFile(string siteUrl, ImmutableList<BlogPostInfo> allPosts)
        {
            var xmlDoc = new XmlDocument();
            var urlSetElement = xmlDoc.CreateElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xmlDoc.AppendChild(urlSetElement);

            var lastModifiedString = DateTime.Now.ToString("yyyy-MM-dd");
            foreach (var post in allPosts)
            {
                var urlNode = xmlDoc.CreateElement("url", "http://www.sitemaps.org/schemas/sitemap/0.9");

                var locationNode = xmlDoc.CreateElement("loc");
                locationNode.InnerText = siteUrl + "blog/posts/" + post.FileNameWithoutExtension;

                var lastModifiedNode = xmlDoc.CreateElement("lastmod");
                lastModifiedNode.InnerText = lastModifiedString;

                urlNode.AppendChild(locationNode);
                urlNode.AppendChild(lastModifiedNode);
                urlSetElement.AppendChild(urlNode);
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
                    string postContent = File.ReadAllText(x);
                    BlogPostEntry blogEntry = parser.ParseFromMarkdown(postContent);
                    return new BlogPostInfo(fileNameWithoutExtension, blogEntry);
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
