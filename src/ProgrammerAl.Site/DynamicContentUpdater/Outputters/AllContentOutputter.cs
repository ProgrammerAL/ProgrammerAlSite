using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.DynamicContentUpdater;

namespace DynamicContentUpdater.Outputters;

public class AllContentOutputter
{
    public void Output(RuntimeConfig runtimeConfig, string contentPath)
    {
        Console.WriteLine($"Copying all files to {runtimeConfig.OutputDirectory}...");

        foreach (var file in Directory.GetFiles(contentPath, "*.*", SearchOption.AllDirectories))
        {
            var filePathWithoutContentPath = file.Substring(contentPath.Length);
            var destinationFile = $"{runtimeConfig.OutputDirectory}/{filePathWithoutContentPath}";

            var destinationDir = new FileInfo(destinationFile).DirectoryName;
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
