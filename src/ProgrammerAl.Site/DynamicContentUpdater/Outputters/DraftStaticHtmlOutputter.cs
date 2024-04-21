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

public class DraftStaticHtmlOutputter
{
    public async ValueTask OutputAsync(RuntimeConfig runtimeConfig, string pathToTemplatesDir, ImmutableArray<PostEntry> allPosts)
    {
        await Console.Out.WriteLineAsync("Outputting static html draft files...");

        //Load the static templating engine
        var engine = new RazorLightEngineBuilder()
          .UseFileSystemProject(pathToTemplatesDir)
          .UseMemoryCachingProvider()
          .Build();

        //Create static html files for each blog post entry
        foreach (var post in allPosts)
        {
            var staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", post);
            staticHtml = staticHtml.Replace("__StorageSiteUrl__", runtimeConfig.StorageUrl);

            string outputFilePath = $"{runtimeConfig.OutputDirectory}/Posts/{post.TitleLink}/{PostEntry.HtmlFileName}";

            var destinationDir = new FileInfo(outputFilePath).DirectoryName;
            if (!Directory.Exists(destinationDir))
            {
                Console.WriteLine($"\tCreating directory '{destinationDir}'");
                Directory.CreateDirectory(destinationDir);
            }

            Console.WriteLine($"Writing to file: {outputFilePath}");
            File.WriteAllText(outputFilePath, staticHtml);
        }

        await Console.Out.WriteLineAsync($"Completed outputting static html draft files...");
    }

    private static void EnsureOutputDirectoryExists(string outputfolderPath)
    {
        if (!Directory.Exists(outputfolderPath))
        {
            _ = Directory.CreateDirectory(outputfolderPath);
        }
    }
}
