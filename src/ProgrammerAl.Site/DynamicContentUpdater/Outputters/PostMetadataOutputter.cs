using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;
using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class PostMetadataOutputter
{
    public void Output(RuntimeConfig runtimeConfig, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine("Outputting post metadata files...");

        //Create static html files for each blog post entry
        foreach (var post in allPosts)
        {
            _ = post.TryGetComicSvgLink(out var comicLink);
            var presentations = post.Presentations.Select(x => new PostMetadata.PresentationData(x.Id, x.SlidesRootUrl)).ToImmutableArray();
            var metadata = new PostMetadata(
                 Title: post.TitleHumanReadable,
                 ComicImageLink: comicLink,
                 ReleaseDate: post.ReleaseDate,
                 Presentations: presentations);

            var outputDir = $"{runtimeConfig.OutputDirectory}/Posts/{post.TitleLink}";
            OutputUtils.WriteOutFileAsJson(metadata, outputDir, PostMetadata.FileName);
        }

        Console.WriteLine($"Completed outputting post metadata files...");
    }
}
