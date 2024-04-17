using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicContentUpdater.Entities;

using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class PostMetadataOutputter
{
    public void Output(string contentPath, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine("Outputting post metadata files...");

        string postsFolderPath = $"{contentPath}/Posts";
        EnsureOutputDirectoryExists(postsFolderPath);

        //Create static html files for each blog post entry
        foreach (var post in allPosts)
        {
            var comicLink = post.TryGetComicLink(out var outComicLink) ? outComicLink : null;
            var metadata = new PostMetadata(
                title: post.TitleHumanReadable,
                comicImageLink: comicLink);

            var postDir = $"{postsFolderPath}/{post.TitleLink}";

            OutputUtils.WriteOutFileAsJson(metadata, postDir, PostMetadata.FileName);
        }

        Console.WriteLine($"Completed outputting post metadata files...");
    }

    private static void EnsureOutputDirectoryExists(string outputfolderPath)
    {
        if (!Directory.Exists(outputfolderPath))
        {
            _ = Directory.CreateDirectory(outputfolderPath);
        }
    }
}
