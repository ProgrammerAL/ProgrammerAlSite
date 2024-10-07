using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var destinationDir = new FileInfo(destinationFile).DirectoryName!;
            if (!Directory.Exists(destinationDir))
            {
                Console.WriteLine($"\tCreating directory '{destinationDir}'");
                Directory.CreateDirectory(destinationDir);
            }

            var svgText = File.ReadAllText(sourceFilePath);
            var sanitizedSvg = ReplaceSvgVariablesWithDefaultValues(svgText);

            var encodedSvg = Encoding.UTF8.GetBytes(sanitizedSvg);
            using var memoryStream = new MemoryStream(encodedSvg);
            var svgDocument = SvgDocument.Open<SvgDocument>(memoryStream);

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

    /// <summary>
    /// Replaces any SVG variables inside the Style element with the default values
    /// This is because PNG files can't use CSS variables, so we need to force it to use the default values set
    /// </summary>
    private string ReplaceSvgVariablesWithDefaultValues(string svgText)
    {
        //The :root section of the svg file contains the variables that need to be replaced
        var rootStyleStartIndex = svgText.IndexOf(":root");
        var endIndex = svgText.IndexOf("}", rootStyleStartIndex);

        var rootStyle = svgText.Substring(rootStyleStartIndex, endIndex - rootStyleStartIndex + 1);

        var variables = new Dictionary<string, string>();

        //Find all the variables in the :root section
        var count = 0;
        var index = 0;
        while (count < 1_000)//Sanity check to prevent infinite loop
        {
            count++;
            index = rootStyle.IndexOf("--", index);
            if (index == -1)
            {
                break;
            }

            var endIndexOfVariable = rootStyle.IndexOf(":", index);
            var variableName = rootStyle.Substring(index, endIndexOfVariable - index).Trim();
            var variableValueStartIndex = endIndexOfVariable + 2;
            var variableValueEndIndex = rootStyle.IndexOf("\n", variableValueStartIndex);
            var variableValue = rootStyle.Substring(variableValueStartIndex, variableValueEndIndex - variableValueStartIndex).Trim();

            variables[variableName] = variableValue;
            index = variableValueStartIndex;
        }

        //In the rest of the style, replace the variables with their values
        foreach (var variable in variables)
        {
            var variableKeyText = $"var({variable.Key})";
            var variableValueText = variable.Value;
            svgText = svgText.Replace(variableKeyText, variableValueText);
        }

        return svgText;
    }
}
