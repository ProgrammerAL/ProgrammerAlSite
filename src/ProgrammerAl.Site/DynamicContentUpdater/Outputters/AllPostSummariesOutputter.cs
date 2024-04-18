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
                    string comicImageLink = null;

                    return new PostSummary(
                        titleHumanReadable: x.TitleHumanReadable,
                        titleLink: x.TitleLink,
                        postedDate: x.ReleaseDate,
                        firstParagraph: x.FirstParagraphHtml,
                        postNumber: x.PostNumber,
                        tags: x.Tags.ToArray(),
                        comicImageLink: x.TryGetComicLink(out comicImageLink) ? comicImageLink : null
                    );
                })
                .ToArray();

        OutputUtils.WriteOutFileAsJson(allSummaries, runtimeConfig.OutputDirectory, PostSummary.AllPostSummariesFile);

        Console.WriteLine($"Completed output of {PostSummary.AllPostSummariesFile}...");
    }
}
