using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicContentUpdater.Utilities;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;

using RazorLight;

namespace DynamicContentUpdater.Outputters;

public class PostStaticHtmlOutputter
{
    public async ValueTask OutputAsync(RuntimeConfig runtimeConfig, string pathToTemplatesDir, ImmutableArray<PostEntry> allPosts)
    {
        await Console.Out.WriteLineAsync("Outputting static html post files...");

        //Load the static templating engine
        var engine = new RazorLightEngineBuilder()
          .UseFileSystemProject(pathToTemplatesDir)
          .UseMemoryCachingProvider()
          .Build();

        //Create static html files for each blog post entry
        foreach (var post in allPosts)
        {
            var staticHtml = await engine.CompileRenderAsync<PostEntry>("Post.cshtml", post);
            staticHtml = HtmlModificationUtility.ReplacePlaceholders(staticHtml, runtimeConfig, post);

            string outputFilePath = $"{runtimeConfig.OutputDirectory}/Posts/{post.TitleLink}/{PostEntry.HtmlFileName}";
            Console.WriteLine($"Writing to file: {outputFilePath}");
            File.WriteAllText(outputFilePath, staticHtml);
        }

        await Console.Out.WriteLineAsync($"Completed outputting static html files...");
    }
}
