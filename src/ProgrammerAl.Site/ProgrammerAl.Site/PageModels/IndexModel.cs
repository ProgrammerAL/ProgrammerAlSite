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
        protected FileDownloader FileDownloader { get; set; }

        public RecentData Recents { get; set; }

        public bool IsLoadingRecents { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoadingRecents = true;
            StateHasChanged();

            var recentDataContent = await FileDownloader.DownloadFileFromSiteContentAsync(PostSummary.RecentSummariesFile, "*/*");
            Recents = await JsonSerializer.DeserializeAsync<RecentData>(recentDataContent);

            IsLoadingRecents = false;
            StateHasChanged();

            await base.OnInitializedAsync();
        }
    }
}
