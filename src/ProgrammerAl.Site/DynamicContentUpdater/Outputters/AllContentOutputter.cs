using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Razor.Language;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;
using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class AllContentOutputter
{
    public void Output(RuntimeConfig runtimeConfig, string contentPath)
    {
        Console.WriteLine($"Copying all files to {runtimeConfig.OutputDirectory}...");

        foreach (var file in Directory.GetFiles(contentPath, "*.*", SearchOption.AllDirectories))
        {
            var destinationFile = $"{runtimeConfig.OutputDirectory}/{Path.GetFileName(file)}";

            var destinationDir = new FileInfo(destinationFile).Directory.FullName;
            if (!Directory.Exists(destinationDir))
            {
                Console.WriteLine($"\tCreating directory '{destinationDir}'");
                Directory.CreateDirectory(destinationDir);
            }

            Console.WriteLine($"\tCopying '{file}' to '{destinationFile}'");
            File.Copy(file, destinationFile, true);
        }


        Console.WriteLine($"Completed copying all files to {runtimeConfig.OutputDirectory}...");
    }
}
