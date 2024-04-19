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
            staticHtml = staticHtml.Replace("__StorageSiteUrl__", runtimeConfig.StorageUrl);

            string outputFilePath = $"{runtimeConfig.OutputDirectory}/Posts/{post.TitleLink}/{PostEntry.HtmlFileName}";
            File.WriteAllText(outputFilePath, staticHtml);
        }

        await Console.Out.WriteLineAsync($"Completed outputting static html files...");
    }
}
