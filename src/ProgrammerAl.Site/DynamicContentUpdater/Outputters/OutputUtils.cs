using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DynamicContentUpdater.Outputters;

public static class OutputUtils
{
    public static void WriteOutFileAsJson<T>(T obj, string outputDirectoryPath, string fileName)
    {
        EnsureOutputDirectoryExists(outputDirectoryPath);

        string objJson = JsonConvert.SerializeObject(obj);
        string outputFilePath = Path.Combine(outputDirectoryPath, fileName);

        Console.WriteLine($"Writing to file: {outputFilePath}");

        File.WriteAllText(outputFilePath, objJson);
    }

    private static void EnsureOutputDirectoryExists(string outputfolderPath)
    {
        if (!Directory.Exists(outputfolderPath))
        {
            _ = Directory.CreateDirectory(outputfolderPath);
        }
    }
}
