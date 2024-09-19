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
    public int Index { get; set; }

    private MarkupString SlidesHtml { get; set; }
    private PostData? PostData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(PostUrl) && Index > -1)
        {
            PostData = await PostDataProvider.GetPostAsync(PostUrl);

            if (PostData is object
                && Index < PostData.Metadata.PresentationSlideUrls.Length)
            {
                var slidesUrl = PostData.Metadata.PresentationSlideUrls[Index];
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
