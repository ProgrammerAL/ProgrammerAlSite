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
                    string comicImageLink = null;

                    return new PostSummary(
                        titleHumanReadable: x.TitleHumanReadable,
                        titleLink: x.TitleLink,
                        postedDate: x.ReleaseDate,
                        firstParagraph: x.FirstParagraphHtml,
                        postNumber: x.PostNumber,
                        tags: x.Tags.ToArray(),
                        comicImageLink: x.TryGetComicSvgLink(out comicImageLink) ? comicImageLink : null
                    );
                })
                .ToArray();

        var recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };
        OutputUtils.WriteOutFileAsJson(recentData, runtimeConfig.OutputDirectory, PostSummary.RecentSummariesFile);

        Console.WriteLine($"Completed output of {PostSummary.RecentSummariesFile}");
    }
}
