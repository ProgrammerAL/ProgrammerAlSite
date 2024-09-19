using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities;

namespace ProgrammerAl.Site.Pages;

public partial class PresentationSlides : ComponentBase
{
    [Inject, NotNull]
    private PostDataProvider? PostDataProvider { get; set; }

    [Inject, NotNull]
    private FileDownloader? FileDownloader { get; set; }

    [Parameter]
    public string? PostUrl { get; set; }

    [Parameter]
    public int? Index { get; set; }

    private MarkupString SlidesHtml { get; set; }
    private PostData? PostData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //var client = new HttpClient();
        //var result = await client.GetAsync("https://raw.githubusercontent.com/ProgrammerAL/Presentations-2024/main/devops-days-tampa-bay-2024/presentation.html");
        //var content = await result.Content.ReadAsStringAsync();
        //PostHtml = new MarkupString(content);
        //await InvokeAsync(StateHasChanged);

        if (!string.IsNullOrWhiteSpace(PostUrl))
        {
            PostData = await PostDataProvider.GetPostAsync(PostUrl);

            if (PostData is object
                && Index.HasValue
                    && Index > 0
                    && Index < PostData.Metadata.PresentationSlideUrls.Length)
            {
                var slidesUrl = PostData.Metadata.PresentationSlideUrls[Index.Value];
                var slidesHtml = await FileDownloader.DownloadFileTextFromSiteContentAsync(slidesUrl, "*/*");
                if (!string.IsNullOrWhiteSpace(slidesHtml))
                {
                    SlidesHtml = new MarkupString(slidesHtml);
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        await base.OnInitializedAsync();
    }

    private bool ShouldPostRequestFeedback()
    {
        if (PostData is null)
        {
            return false;
        }

        var feedbackStartDate = PostData.Metadata.ReleaseDate.AddDays(-3);
        var feedbackEndDate = PostData.Metadata.ReleaseDate.AddDays(7);

        var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (todayDate < feedbackStartDate
            || todayDate > feedbackEndDate)
        {
            return false;
        }

        return true;
    }
}
