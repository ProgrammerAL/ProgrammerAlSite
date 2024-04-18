using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;

using RazorLight;

namespace DynamicContentUpdater.Outputters;

public class PostStaticMetaTagFilesOutputter
{
    public async ValueTask OutputAsync(RuntimeConfig runtimeConfig, string contentPath, string fullPathToTemplates, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting meta tag html files...");

        //Load the static templating engine
        var engine = new RazorLightEngineBuilder()
          .UseFileSystemProject(fullPathToTemplates)
          .UseMemoryCachingProvider()
          .Build();

        string postsFolderPath = $"{contentPath}/Posts";
        EnsureOutputDirectoryExists(postsFolderPath);

        //TODO: This only outputs for posts
        //      Need to do the same for Comic pages too
        foreach (var post in allPosts)
        {
            var staticHtml = await engine.CompileRenderAsync<PostEntry>("MetaTags.cshtml", post);
            staticHtml = staticHtml.Replace("__StorageSiteUrl__", runtimeConfig.StorageUrl);

            string outputFilePath = $"{postsFolderPath}/{post.TitleLink}/{PostEntry.HtmlFileName}";
            File.WriteAllText(outputFilePath, staticHtml);
        }

        Console.WriteLine($"Completed output of meta tag html files");
    }

    private static void EnsureOutputDirectoryExists(string outputfolderPath)
    {
        if (!Directory.Exists(outputfolderPath))
        {
            _ = Directory.CreateDirectory(outputfolderPath);
        }
    }
}
