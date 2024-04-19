using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Pulumi.Automation;

namespace ProgrammerAl.Site.ContentUploader
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            RuntimeConfig runtimeConfig = null;

            _ = Parser.Default.ParseArguments<RuntimeConfig>(args)
                .WithParsed(opts => runtimeConfig = opts)
                .WithNotParsed((errs) =>
                {
                    Console.WriteLine(errs);
                    return;
                });

            //var runtimeConfig = new RuntimeConfig
            //{
            //    StorageUrl = "https://my-test.whatever.com",
            //    ContentDirectory = "C:/GitHub/ProgrammerAl/ProgrammerAlSite/src/ProgrammerAl.Site.Content/Posts"
            //    //ContentDirectory = "C:\\GitHub\\ProgrammerAl\\ProgrammerAlSite\\src\\ProgrammerAl.Site.Content\\Posts"
            //};

            //We assume there's no slash in the string, so remove it if it's there as a just in case
            runtimeConfig.ContentDirectory = runtimeConfig.ContentDirectory.TrimEnd('/', '\\');

            var stackArgs = new LocalProgramArgs(runtimeConfig.PulumiStackName, runtimeConfig.PulumiStackPath);
            var stack = await LocalWorkspace.SelectStackAsync(stackArgs);
            var outputs = await stack.GetOutputsAsync();

            var storageApiEndpointOutput = outputs["StorageApiHttpsEndpoint"];
            var storageApiAdminAuthTokenOutput = outputs["StorageApiAdminAuthToken"];

            var storageApiEndpoint = storageApiEndpointOutput.Value.ToString();
            await Console.Out.WriteLineAsync($"Pushing files to: {storageApiEndpoint}");

            var adminToken = storageApiAdminAuthTokenOutput.Value.ToString();
            //await Console.Out.WriteLineAsync($"Pushing files using admin token: {adminToken}");

            var client = new HttpClient();
            client.BaseAddress = new Uri(storageApiEndpoint);
            client.DefaultRequestHeaders.Add("x-admin-token", adminToken);

            //Length of the initial directory, plus 1 for the slash
            var startIndex = runtimeConfig.ContentDirectory.Length + 1;

            var contentFiles = Directory.GetFiles(runtimeConfig.ContentDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in contentFiles)
            {
                var storagePath = filePath
                                    .Replace('\\', '/')
                                    .Replace("//", "/")
                                    .Substring(startIndex);
                var pushPath = $"/storage/{storagePath}?action=store-object";

                await Console.Out.WriteLineAsync($"Processing file {filePath}");
                await Console.Out.WriteLineAsync($"\tUploading to {pushPath}");

                var request = new HttpRequestMessage(HttpMethod.Put, pushPath);
                using var fileStream = File.Open(filePath, FileMode.Open);
                request.Content = new StreamContent(fileStream);

                var result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    await Console.Out.WriteLineAsync($"\t\tResult Success");
                }
                else
                {
                    var resultContent = await result.Content.ReadAsStringAsync();
                    await Console.Out.WriteLineAsync($"\t\tResult Error {result.StatusCode}. {resultContent}");
                }
            }
        }
    }
}
