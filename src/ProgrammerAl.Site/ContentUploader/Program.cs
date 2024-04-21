using System.IO;
using System.Linq;
using System.Collections.Immutable;
using CommandLine;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Pulumi.Automation;
using System.Collections.Generic;

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
            var startIndex = runtimeConfig.ContentDirectory.Length;

            var contentFiles = Directory.GetFiles(runtimeConfig.ContentDirectory, "*.*", SearchOption.AllDirectories);
            var uploadTasks = new List<Task>(contentFiles.Length);
            foreach (var filePath in contentFiles)
            {
                var uploadTask = UploadFileAsync(filePath, startIndex, client);
                uploadTasks.Add(uploadTask);
            }

            await Task.WhenAll(uploadTasks);
            await Console.Out.WriteLineAsync("Done uploading all files...");
        }

        private static async Task UploadFileAsync(string filePath, int rootPathStartIndex, HttpClient httpClient)
        {
            var storagePath = filePath
                    .Replace('\\', '/')
                    .Replace("//", "/")
                    .Substring(rootPathStartIndex);
            var pushPath = $"/storage/{storagePath}?action=store-object";

            Console.WriteLine($"Processing file {filePath}");
            Console.WriteLine($"\tUploading to {pushPath}");

            var request = new HttpRequestMessage(HttpMethod.Put, pushPath);
            using var fileStream = File.Open(filePath, FileMode.Open);
            request.Content = new StreamContent(fileStream);

            var result = await httpClient.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine($"\t\tResult Success");
            }
            else
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"\t\tResult Error {result.StatusCode}. {resultContent}");
            }
        }
    }
}
