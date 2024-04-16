using ProgrammerAl.Site.Utilities;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using System;
using RazorLight;
using System.Threading.Tasks;
using Markdig;
using DynamicContentUpdater;
using DynamicContentUpdater.Entities;
using DynamicContentUpdater.Outputters;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        private const string ComicsFile = "Comics.json";

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
            var sitemapFilePath = parsedArgs.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";

            var allPosts = LoadAllPostsOrderedByDate(contentPath, parser);

            new RecentPostsOutputter().Output(contentPath, allPosts);
            new TagLinksOutputter().Output(contentPath, allPosts);
            new SiteMapOutputter().Output(contentPath, sitemapFilePath, allPosts);

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
            foreach (var post in allPosts)
            {
                string staticHtml;
                if (post.TryGetComicLink(out _))
                {
                    staticHtml = await engine.CompileRenderAsync<PostEntry>("ComicPost.cshtml", post);
                }
                else
                {
                    staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", post);
                }

                string outputFilePath = $"{outputfolderPath}/{post.TitleLink}/post.html";
                File.WriteAllText(outputFilePath, staticHtml);
            }
        }

        public static ImmutableArray<PostEntry> LoadAllPostsOrderedByDate(string contentPath, PostParser parser)
        {
            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

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
                var parsedEntry = parser.ParseFromMarkdown(postContent);
                return new
                {
                    PostName = postName,
                    PostDate = postDate,
                    ParsedEntry = parsedEntry
                };
            })
                .OrderBy(x => x.PostDate)//All blog posts start with the date they were posted. Order them so the oldest is first
                .Select((x, i) =>
                {
                    var postNumber = i + 1;

                    return new PostEntry(
                        titleHumanReadable: x.ParsedEntry.Title,
                        titleLink: x.PostName,//public url for the post, just the full name of it. ie 20210101-Title
                        releaseDate: x.PostDate,
                        tags: x.ParsedEntry.Tags.ToImmutableArray(),
                        postMarkdown: x.ParsedEntry.Post,
                        postHtml: Markdig.Markdown.ToHtml(x.ParsedEntry.Post, pipeline: markdownPipeline),
                        firstParagraphHtml: Markdig.Markdown.ToHtml(x.ParsedEntry.FirstParagraph, pipeline: markdownPipeline),
                        postNumber: postNumber
                    );
                })
                .ToImmutableArray();
        }
    }
}
