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
            var comicLink = post.TryGetComicSvgLink(out var outComicLink) ? outComicLink : null;
            var metadata = new PostMetadata(
                title: post.TitleHumanReadable,
                comicImageLink: comicLink);

            var outputDir = $"{runtimeConfig.OutputDirectory}/Posts/{post.TitleLink}";
            OutputUtils.WriteOutFileAsJson(metadata, outputDir, PostMetadata.FileName);
        }

        Console.WriteLine($"Completed outputting post metadata files...");
    }
}
