using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using System;
using System.Threading.Tasks;
using Markdig;
using DynamicContentUpdater;
using DynamicContentUpdater.Outputters;
using ProgrammerAl.Site.PostDataEntities;
using System.Diagnostics;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            RuntimeConfig runtimeConfig = null;

            _ = Parser.Default.ParseArguments<RuntimeConfig>(args)
                .WithParsed(opts => runtimeConfig = opts)
                .WithNotParsed((errs) =>
                {
                    Console.WriteLine(errs);
                    return;
                });

            var parser = new PostParser(runtimeConfig);

            var contentPath = runtimeConfig.AppRootPath + "/ProgrammerAl.Site.Content";
            var sitemapFilePath = runtimeConfig.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";
            var pathToTemplatesDir = runtimeConfig.AppRootPath + "/ProgrammerAl.Site/DynamicContentUpdater/StaticTemplates";

            var allPosts = LoadAllPostsOrderedByDate(contentPath, parser);

            new SiteMapOutputter().Output(sitemapFilePath, allPosts);

            new RecentPostsOutputter().Output(runtimeConfig, allPosts);
            new AllPostSummariesOutputter().Output(runtimeConfig, allPosts);
            new TagLinksOutputter().Output(runtimeConfig, allPosts);
            new PostMetadataOutputter().Output(runtimeConfig, allPosts);

            new AllContentOutputter().Output(runtimeConfig, contentPath);
            await new PostStaticHtmlOutputter().OutputAsync(runtimeConfig, pathToTemplatesDir, allPosts);
            await new PostStaticMetaTagFilesOutputter().OutputAsync(runtimeConfig, pathToTemplatesDir, allPosts);
        }

        public static ImmutableArray<PostEntry> LoadAllPostsOrderedByDate(string contentPath, PostParser parser)
        {
            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            string blogPostsFolderPath = contentPath + "/Posts";
            string[] blogPostFolders = Directory.GetDirectories(blogPostsFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            return blogPostFolders
                .Where(x => !x.Contains("draft__", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
            {
                var dirInfo = new DirectoryInfo(x);
                var postFilePath = $"{x}/post.md";
                var comicPath = $"{x}/comic.svg";
                var postName = dirInfo.Name; // Name of post is the name of the folder

                if (!File.Exists(postFilePath))
                {
                    throw new Exception("$Could not find post file: {postFilePath}");
                }

                var postDirectoryLocalPath = dirInfo.FullName;
                var postDateString = postName.Substring(0, 8);
                var postDate = DateOnly.ParseExact(postDateString, "yyyyMMdd");
                string postContent = File.ReadAllText(postFilePath);
                var parsedEntry = parser.ParseFromMarkdown(postContent);

                return new
                {
                    PostDirectoryLocalPath = postDirectoryLocalPath,
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
                        postDirectoryLocalPath: x.PostDirectoryLocalPath,
                        titleHumanReadable: x.ParsedEntry.Title,
                        titleLink: x.PostName,//public url for the post, just the full name of it. ie 20210101-Title
                        releaseDate: x.PostDate,
                        tags: x.ParsedEntry.Tags.ToImmutableArray(),
                        postMarkdown: x.ParsedEntry.Post,
                        postHtml: Markdown.ToHtml(x.ParsedEntry.Post, pipeline: markdownPipeline),
                        firstParagraphHtml: Markdown.ToHtml(x.ParsedEntry.FirstParagraph, pipeline: markdownPipeline),
                        postNumber: postNumber
                    );
                })
                .ToImmutableArray();
        }
    }
}
