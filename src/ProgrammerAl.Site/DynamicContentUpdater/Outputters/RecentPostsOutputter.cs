using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;
using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class RecentPostsOutputter
{
    private const int FrontPageBlogsDisplayed = 5;

    public void Output(RuntimeConfig runtimeConfig, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting {PostSummary.RecentSummariesFile}...");

        PostSummary[] mostRecentBlogPosts = allPosts
                .OrderByDescending(x => x.PostNumber)
                .Take(FrontPageBlogsDisplayed)
                .Select(x =>
                {
                    string? comicImageLink = null;
                    _ = x.TryGetComicSvgLink(out comicImageLink);

                    return new PostSummary(
                        TitleHumanReadable: x.TitleHumanReadable,
                        TitleLink: x.TitleLink,
                        PostedDate: x.ReleaseDate,
                        FirstParagraph: x.FirstParagraphHtml,
                        PostNumber: x.PostNumber,
                        Tags: x.Tags.ToArray(),
                        ComicImageLink: comicImageLink
                    );
                })
                .ToArray();

        var recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };
        OutputUtils.WriteOutFileAsJson(recentData, runtimeConfig.OutputDirectory, PostSummary.RecentSummariesFile);

        Console.WriteLine($"Completed output of {PostSummary.RecentSummariesFile}");
    }
}
