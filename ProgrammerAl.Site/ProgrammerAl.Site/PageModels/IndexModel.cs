using Microsoft.AspNetCore.Components;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.PageModels
{
    public class IndexModel : ComponentBase
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IConfig Config { get; set; }

        protected RecentData Recents { get; set; }

        protected override async Task OnInitAsync()
        {
            var downloader = new FileDownloader();
            var recentDataText = await downloader.DownloadFileFromSiteContentAsync(HttpClient, Config, "RecentData.json", "*/*");
            Recents = Microsoft.JSInterop.Json.Deserialize<RecentData>(recentDataText);

            await base.OnInitAsync();
        }
    }
}
