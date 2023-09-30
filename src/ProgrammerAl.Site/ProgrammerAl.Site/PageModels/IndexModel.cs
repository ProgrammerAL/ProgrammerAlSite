using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.PageModels
{
    public class IndexModel : ComponentBase
    {
        [Inject]
        protected IConfig Config { get; set; }

        public RecentData Recents { get; set; }

        public bool IsLoadingRecents { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoadingRecents = true;
            StateHasChanged();

            var downloader = new FileDownloader();
            var recentDataContent = await downloader.DownloadFileFromSiteContentAsync(Config, "RecentData.json", "*/*");
            Recents = await JsonSerializer.DeserializeAsync<RecentData>(recentDataContent);

            IsLoadingRecents = false;
            StateHasChanged();

            await base.OnInitializedAsync();
        }
    }
}
