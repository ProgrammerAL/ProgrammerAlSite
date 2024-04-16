using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicContentUpdater.Entities;

using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class AllPostSummariesOutputter
{
    private const string BlogPostsFile = "BlogPosts.json";

    public void Output(string contentPath, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting {BlogPostsFile}...");

        var allSummaries = allPosts
                .OrderByDescending(x => x.PostNumber)
                .Select(x =>
                {
                    string comicImageLink = null;

                    return new PostSummary(
                        title: x.TitleHumanReadable,
                        postedDate: x.ReleaseDate,
                        titleLink: x.TitleLink,
                        firstParagraph: x.FirstParagraphHtml,
                        postNumber: x.PostNumber,
                        tags: x.Tags.ToArray(),
                        comicImageLink: x.TryGetComicLink(out comicImageLink) ? comicImageLink : null
                    );
                })
                .ToArray();

        OutputUtils.WriteOutFileAsJson(allSummaries, contentPath, BlogPostsFile);

        Console.WriteLine($"Completed output of {BlogPostsFile}...");
    }
}
