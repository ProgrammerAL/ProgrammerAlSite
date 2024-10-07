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

public class AllPostSummariesOutputter
{
    public void Output(RuntimeConfig runtimeConfig, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting {PostSummary.AllPostSummariesFile}...");

        var allSummaries = allPosts
                .OrderByDescending(x => x.PostNumber)
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

        OutputUtils.WriteOutFileAsJson(allSummaries, runtimeConfig.OutputDirectory, PostSummary.AllPostSummariesFile);

        Console.WriteLine($"Completed output of {PostSummary.AllPostSummariesFile}...");
    }
}
