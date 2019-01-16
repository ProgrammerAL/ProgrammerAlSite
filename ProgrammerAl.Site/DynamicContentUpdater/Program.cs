using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using Newtonsoft.Json;
using System;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        private const string RecentDataFile = "RecentData.json";
        private const string BlogPostsFile = "BlogPosts.json";
        private const int FrontPageBlogsDisplayed = 5;

        static void Main(string[] args)
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

            ImmutableList<BlogPostInfo> allPosts = LoadAllBlogPostInfo(parsedArgs.ContentPath, parser);

            ImmutableList<BlogPostInfo> parsedBlogEntries = allPosts
                .OrderBy(x => x.FileNameWithoutExtension)//All blog posts start with a number. So the higher the number, the newer the post
                .ToImmutableList();

            int blogPostNumber = 1;
            ImmutableList<BlogPostSummary> allBlogPostSummaries = parsedBlogEntries.Select(x => new BlogPostSummary
            {
                Title = x.Entry.Title,
                PostedDate = x.Entry.ReleaseDate,
                FirstParagraph = x.Entry.FirstParagraph,
                PostNumber = blogPostNumber++,
                TitleLink = x.FileNameWithoutExtension,
            }).ToImmutableList();

            ImmutableList<BlogPostSummary> mostRecentBlogPosts = allBlogPostSummaries
                                        .OrderByDescending(x => x.PostNumber)
                                        .Take(FrontPageBlogsDisplayed)
                                        .ToImmutableList();

            RecentData recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };

            WriteOutFile(recentData, parsedArgs.ContentPath, "RecentData.json");
            WriteOutFile(allBlogPostSummaries, parsedArgs.ContentPath, "BlogPosts.json");

            //TODO: Output markdown files as HTML
            foreach (var blogEntry in parsedBlogEntries)
            {
            }
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

        private static void WriteOutFile<T>(T obj, string contentPath, string fileName)
        {
            string objJson = JsonConvert.SerializeObject(obj);
            string outputFilePath = Path.Combine(contentPath, fileName);
            File.WriteAllText(outputFilePath, objJson);
        }
    }
}
