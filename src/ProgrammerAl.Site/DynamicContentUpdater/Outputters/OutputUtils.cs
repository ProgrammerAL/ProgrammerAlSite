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
    public static void WriteOutFileAsJson<T>(T obj, string contentPath, string fileName)
    {
        string objJson = JsonConvert.SerializeObject(obj);
        string outputFilePath = Path.Combine(contentPath, fileName);
        File.WriteAllText(outputFilePath, objJson);
    }
}
