﻿using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using System;
using System.Threading.Tasks;
using Markdig;
using DynamicContentUpdater;
using DynamicContentUpdater.Outputters;
using ProgrammerAl.Site.PostDataEntities;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    public record PostEntries(ImmutableArray<PostEntry> Posts, ImmutableArray<PostEntry> Drafts);
    partial class Program
    {
        static async Task Main(string[] args)
        {
            RuntimeConfig? runtimeConfig = null;

            _ = Parser.Default.ParseArguments<RuntimeConfig>(args)
                .WithParsed(opts => runtimeConfig = opts)
                .WithNotParsed((errs) =>
                {
                    Console.WriteLine(errs);
                    return;
                });

            if (runtimeConfig is null)
            {
                Console.WriteLine("Can't continue, no runtime config");
                return;
            }

            var parser = new PostParser(runtimeConfig);

            var contentPath = runtimeConfig.AppRootPath + "/ProgrammerAl.Site.Content";
            var sitemapFilePath = runtimeConfig.AppRootPath + "/ProgrammerAl.Site/ProgrammerAl.Site/wwwroot/sitemap.xml";
            var pathToTemplatesDir = runtimeConfig.AppRootPath + "/ProgrammerAl.Site/DynamicContentUpdater/StaticTemplates";

            var postEntries = LoadAllPostEntries(contentPath, parser);

            //Make sure this one is first so we don't double-copy files
            new AllContentOutputter().Output(runtimeConfig, contentPath);
            new SvgToPngOutputter().Output(runtimeConfig, contentPath);

            new SiteMapOutputter().Output(sitemapFilePath, postEntries.Posts);

            new RecentPostsOutputter().Output(runtimeConfig, postEntries.Posts);
            new AllPostSummariesOutputter().Output(runtimeConfig, postEntries.Posts);
            new TagLinksOutputter().Output(runtimeConfig, postEntries.Posts);

            //Output the metadata files for the posts and drafts
            new PostMetadataOutputter().Output(runtimeConfig, postEntries.Posts);
            new PostMetadataOutputter().Output(runtimeConfig, postEntries.Drafts);

            //Output the static html files for the posts and drafts
            await new PostStaticHtmlOutputter().OutputAsync(runtimeConfig, pathToTemplatesDir, postEntries.Posts);
            await new PostStaticHtmlOutputter().OutputAsync(runtimeConfig, pathToTemplatesDir, postEntries.Drafts);

            await new PostStaticMetaTagFilesOutputter().OutputAsync(runtimeConfig, pathToTemplatesDir, postEntries.Posts);
            new AllDraftSummariesOutputter().Output(runtimeConfig, postEntries.Drafts);
        }

        public static PostEntries LoadAllPostEntries(string contentPath, PostParser parser)
        {
            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            string blogPostsFolderPath = contentPath + "/Posts";
            string[] blogPostFolders = Directory.GetDirectories(blogPostsFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            var postsFolderEntries = blogPostFolders
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

                var isDraft = postName.Contains("draft_", StringComparison.OrdinalIgnoreCase);

                string postDirectoryLocalPath;
                DateOnly postDate;
                if (isDraft)
                {
                    postDirectoryLocalPath = dirInfo.FullName;
                    postDate = DateOnly.MinValue;
                }
                else
                {
                    postDirectoryLocalPath = dirInfo.FullName;
                    var postDateString = postName.Substring(0, 8);
                    postDate = DateOnly.ParseExact(postDateString, "yyyyMMdd");
                }

                string postContent = File.ReadAllText(postFilePath);
                var parsedEntry = parser.ParseFromMarkdown(postContent, postName);

                return new
                {
                    PostDirectoryLocalPath = postDirectoryLocalPath,
                    PostName = postName,
                    PostDate = postDate,
                    ParsedEntry = parsedEntry,
                    IsDraft = isDraft
                };
            });

            var postEntries = postsFolderEntries
                .Where(x => !x.IsDraft)
                .OrderBy(x => x.PostDate)//All blog posts start with the date they were posted. Order them so the oldest is first
                .Select((x, i) =>
                {
                    var postNumber = i + 1;

                    var presentations = x.ParsedEntry.Presentations.Select(p => new PostEntry.PresentationEntry(p.Id, p.SlidesRootUrl)).ToImmutableArray();
                    var postHtml = Markdown.ToHtml(x.ParsedEntry.Post, pipeline: markdownPipeline);
                    var firstParagraphHtml = Markdown.ToHtml(x.ParsedEntry.FirstParagraph, pipeline: markdownPipeline);

                    return new PostEntry(
                        PostDirectoryLocalPath: x.PostDirectoryLocalPath,
                        TitleHumanReadable: x.ParsedEntry.Title,
                        TitleLink: x.PostName,//public url for the post, just the full name of it. ie draft_20210101-Title
                        ReleaseDate: x.PostDate,
                        Tags: x.ParsedEntry.Tags,
                        Presentations: presentations,
                        PostMarkdown: x.ParsedEntry.Post,
                        PostHtml: postHtml,
                        FirstParagraphHtml: firstParagraphHtml,
                        PostNumber: postNumber,
                        IsDraft: false
                    );
                })
                .ToImmutableArray();

            var draftEntries = postsFolderEntries
                .Where(x => x.IsDraft)
                .Select((x, i) =>
                {
                    var presentations = x.ParsedEntry.Presentations.Select(p => new PostEntry.PresentationEntry(p.Id, p.SlidesRootUrl)).ToImmutableArray();
                    var postHtml = Markdown.ToHtml(x.ParsedEntry.Post, pipeline: markdownPipeline);
                    var firstParagraphHtml = Markdown.ToHtml(x.ParsedEntry.FirstParagraph, pipeline: markdownPipeline);
                    return new PostEntry(
                        PostDirectoryLocalPath: x.PostDirectoryLocalPath,
                        TitleHumanReadable: x.ParsedEntry.Title,
                        TitleLink: x.PostName,//public url for the post, just the full name of it. ie draft_20210101-Title
                        ReleaseDate: x.PostDate,
                        Tags: x.ParsedEntry.Tags,
                        Presentations: presentations,
                        PostMarkdown: x.ParsedEntry.Post,
                        PostHtml: postHtml,
                        FirstParagraphHtml: firstParagraphHtml,
                        PostNumber: -1,
                        IsDraft: true
                    );
                })
                .ToImmutableArray();

            return new PostEntries(postEntries, draftEntries);
        }
    }
}
