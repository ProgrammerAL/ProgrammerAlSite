using Microsoft.AspNetCore.Components;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.PageModels
{
    public class IndexModel : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected IConfig Config { get; set; }

        public RecentData Recents { get; set; }

        protected override async Task OnInitAsync()
        {
            var downloader = new FileDownloader();
            var recentDataText = await downloader.DownloadFileFromSiteContentAsync(HttpClient, Config, "RecentData.json", "*/*");
            Recents = JsonSerializer.Parse<RecentData>(recentDataText);

            await base.OnInitAsync();
        }
    }
}
