using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicContentUpdater.Entities;

using RazorLight;

namespace DynamicContentUpdater.Outputters;

public class PostStaticHtmlOutputter
{
    public async ValueTask OutputAsync(string contentPath, string fullPathToTemplates, ImmutableArray<PostEntry> allPosts)
    {
        await Console.Out.WriteLineAsync("Outputting static html post files...");

        //Load the static templating engine
        var engine = new RazorLightEngineBuilder()
          .UseFileSystemProject(fullPathToTemplates)
          .UseMemoryCachingProvider()
          .Build();

        string outputfolderPath = Path.Combine(contentPath, "Posts");
        EnsureOutputDirectoryExists(outputfolderPath);

        //Create static html files for each blog post entry
        foreach (var post in allPosts)
        {
            var staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", post);

            string outputFilePath = $"{outputfolderPath}/{post.TitleLink}/post.html";
            File.WriteAllText(outputFilePath, staticHtml);

            //Also copy the comic image file to the output folder
            if (post.HasComic)
            {
                var comicOutputFilePath = $"{outputfolderPath}/{post.TitleLink}/comic.svg";
                File.WriteAllText(comicOutputFilePath, post.ComicSvg);
            }

            //TODO: Output all other files too, for example image assets for a blog post
        }

        Console.WriteLine($"Completed outputting static html files...");
    }

    private static void EnsureOutputDirectoryExists(string outputfolderPath)
    {
        if (!Directory.Exists(outputfolderPath))
        {
            _ = Directory.CreateDirectory(outputfolderPath);
        }
    }
}
