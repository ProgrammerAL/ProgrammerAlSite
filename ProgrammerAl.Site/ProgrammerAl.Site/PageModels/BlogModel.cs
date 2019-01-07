using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.PageModels
{
    public class BlogModel : BlazorComponent
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IConfig Config { get; set; }

        protected BlogPostSummary[] BlogPosts { get; set; }

        protected override async Task OnInitAsync()
        {
            var downloader = new FileDownloader();
            var response = await downloader.DownloadFileFromSiteContentAsync(HttpClient, Config, "BlogPosts.json", "application/json");

            //https://developersidequests.blob.core.windows.net/content/BlogPosts.json
            string recentDataText = await HttpClient.GetStringAsync("BlogPosts.json");

            BlogPosts = Microsoft.JSInterop.Json.Deserialize<BlogPostSummary[]>(recentDataText);

            await base.OnInitAsync();
        }
    }
}
