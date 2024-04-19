using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.Config;
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
        [Inject, NotNull]
        private ApiConfig? ApiConfig { get; set; }

        [Inject, NotNull]
        private FileDownloader? FileDownloader { get; set; }

        public RecentData? Recents { get; set; }

        public bool IsLoadingRecents { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoadingRecents = true;
            StateHasChanged();

            var url = $"{ApiConfig.StorageApiBaseEndpoint}/storage/{PostSummary.RecentSummariesFile}";
            var recentDataContent = await FileDownloader.DownloadFileFromSiteContentAsync(url, "*/*");
            Recents = await JsonSerializer.DeserializeAsync<RecentData>(recentDataContent);

            IsLoadingRecents = false;
            StateHasChanged();

            await base.OnInitializedAsync();
        }
    }
}
