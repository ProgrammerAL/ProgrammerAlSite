using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using System.Collections.Generic;
using ProgrammerAl.DeveloperSideQuests.Blog.Utilities;
using System.Net.Http;
using ProgrammerAl.DeveloperSideQuests.Blog.Utilities.Entities.BlobStorage;
using System.Xml.Serialization;
using System.Collections.Immutable;
using ProgrammerAl.DeveloperSideQuests.Blog.Utilities.Entities;
using System.Text;
using Microsoft.WindowsAzure.Storage;

namespace ProgrammerAl.DeveloperSideQuests.SiteContentUpdateFunctions
{
    public static class UpdateBlogListings
    {
        private const string RecentDataFile = "RecentData.json";
        private const string BlogPostsFile = "BlogPosts.json";


        [FunctionName("UpdateBlogListings")]
        public static async Task Run([BlobTrigger("sitecontent/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            if (string.Equals(name, RecentDataFile, StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, BlogPostsFile, StringComparison.OrdinalIgnoreCase))
            {
                //Don't need to run this forever since we add new files later on
                return;
            }

            var config = new Config();
            var parser = new BlogPostParser(config);

            var allPosts = await GetAllPostsFilesAsync();

            var parsedBlogEntries = allPosts
                .Select(x => new { x.BlobUri, Entry = parser.ParseFromMarkdown(x.FileContents) })
                .OrderByDescending(x => x.Entry.Title)//All blob posts start with a number. So the higher the number, the newer the post
                .ToImmutableList();

            int blogPostNumber = 1;
            var mostRecentBlogPosts = parsedBlogEntries.Select(x => new BlogPostSummary
            {
                Title = x.Entry.Title,
                PostedDate = x.Entry.ReleaseDate,
                FirstParagraph = x.Entry.FirstParagraph,
                PostNumber = blogPostNumber++,
                TitleLink = Path.GetFileNameWithoutExtension(x.BlobUri)
            })
            .Take(3)
            .ToArray();

            var recentData = new RecentData { RecentBlogPosts = mostRecentBlogPosts };
            await UploadFile(recentData, "RecentData.json");
            await UploadFile(parsedBlogEntries.Select(x => x.Entry).ToArray(), "BlogPosts.json");
        }

        private static async Task UploadFile<T>(T obj, string blobName)
        {
            var blobJson = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var jsonBytes = Encoding.ASCII.GetBytes(blobJson);

            var container = new CloudBlobContainer(new Uri(@"https://developersidequestssite.blob.core.windows.net/sitecontent"));

            var serverRef = await container.ServiceClient.GetBlobReferenceFromServerAsync(new Uri(@"https://developersidequestssite.blob.core.windows.net/sitecontent/" + blobName));
            if (await serverRef.ExistsAsync())
            {
                Console.WriteLine("Exists");
            }
            else
            {
                Console.WriteLine("Doesn't exist");
            }
            await serverRef.UploadFromByteArrayAsync(jsonBytes, 0, jsonBytes.Length);

            //var blobReference = container.GetBlockBlobReference(blobName);
            //if (!await blobReference.ExistsAsync())
            //{
            //    //Create the file
            //    Console.WriteLine("Doesn't exist");
            //}
            //await blobReference.UploadFromByteArrayAsync(jsonBytes, 0, jsonBytes.Length);
            ////using (var stream = new MemoryStream(jsonBytes))
            ////{
            ////    await blobReference.UploadFromStreamAsync(stream);
            ////}
            ////await blobReference.UploadTextAsync(blobJson);
        }

        public static async Task<List<(string BlobUri, string FileContents)>> GetAllPostsFilesAsync()
        {
            //The files are all public
            var downloader = new FileDownloader();
            var fileResults = new List<(string BlobUri, string FileContents)>();
            using (var httpClient = new HttpClient())
            {
                //Use HTTP GET to List all files in the posts folder
                var blobListingXml = await httpClient.GetStringAsync("https://developersidequestssite.blob.core.windows.net/sitecontent?comp=list&prefix=posts");
                var serializer = new XmlSerializer(typeof(EnumerationResults));
                var reader = new StringReader(blobListingXml);
                var postsListingResults = serializer.Deserialize(reader) as EnumerationResults;

                //Download each file we know about
                foreach (var blob in postsListingResults.Blobs)
                {
                    var fileText = await httpClient.GetStringAsync(blob.Url);
                    fileResults.Add((blob.Url, fileText));
                }
            }

            return fileResults;
        }
    }
}
