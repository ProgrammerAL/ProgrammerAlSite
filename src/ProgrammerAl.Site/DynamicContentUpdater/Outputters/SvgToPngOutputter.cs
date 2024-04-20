using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Forms;

using ProgrammerAl.Site.DynamicContentUpdater;

using Svg;

namespace DynamicContentUpdater.Outputters;

public class SvgToPngOutputter
{
    public void Output(RuntimeConfig runtimeConfig, string contentPath)
    {
        Console.WriteLine("Outputting post svg files to png files...");

        foreach (var sourceFilePath in Directory.GetFiles(contentPath, "*.svg", SearchOption.AllDirectories))
        {
            var filePathWithoutContentPath = sourceFilePath.Substring(contentPath.Length);
            var destinationFile = $"{runtimeConfig.OutputDirectory}/{filePathWithoutContentPath}";
            destinationFile = destinationFile.Substring(0, destinationFile.Length - 4) + ".png";//Replace .svg with .png

            var destinationDir = new FileInfo(destinationFile).DirectoryName;
            if (!Directory.Exists(destinationDir))
            {
                Console.WriteLine($"\tCreating directory '{destinationDir}'");
                Directory.CreateDirectory(destinationDir);
            }

            var svgDocument = SvgDocument.Open(sourceFilePath);

            if (svgDocument == null)
            {
                Console.WriteLine($"Error: Failed to load svg input file: {sourceFilePath}");
                continue;
            }

            using var bitmap = svgDocument.Draw();
            if (bitmap is null)
            {
                Console.WriteLine($"Error: Failed to draw svg file to bitmap: {sourceFilePath}");
                continue;
            }

            bitmap.Save(destinationFile);
        }

        Console.WriteLine($"Completed outputting post svg files to png files");
    }
}
