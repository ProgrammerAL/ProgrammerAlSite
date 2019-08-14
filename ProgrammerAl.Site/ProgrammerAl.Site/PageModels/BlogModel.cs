using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.PageModels
{
    public class BlogModel : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IConfig Config { get; set; }

        protected BlogPostSummary[] BlogPosts { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var downloader = new FileDownloader();
            //https://programmeralsitecontent.blob.core.windows.net/sitecontent/BlogPosts.json
            var recentDataContent = await downloader.DownloadFileFromSiteContentAsync(HttpClient, Config, "BlogPosts.json", "*/*");

            BlogPosts = await JsonSerializer.DeserializeAsync<BlogPostSummary[]>(recentDataContent);

            await base.OnInitializedAsync();
        }
    }
}
