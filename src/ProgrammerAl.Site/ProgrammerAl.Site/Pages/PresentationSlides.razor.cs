using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

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
    public int Id { get; set; }

    private MarkupString SlidesHtml { get; set; }
    private PostData? PostData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(PostUrl) || Id <= 0)
        {
            return;
        }

        PostData = await PostDataProvider.GetPostAsync(PostUrl);

        var presentation = PostData?.Metadata.Presentations.FirstOrDefault(x => x.Id == Id);
        if (presentation is null)
        {
            return;
        }

        var slidesHtml = await FileDownloader.DownloadFileTextFromSiteContentAsync(presentation.SlidesUrl, "*/*");
        if (string.IsNullOrWhiteSpace(slidesHtml))
        {
            return;
        }

        var sanitizedSlidesHtml = SanitizeHtml(presentation, slidesHtml);

        SlidesHtml = new MarkupString(sanitizedSlidesHtml);
        await InvokeAsync(StateHasChanged);

        await base.OnInitializedAsync();
    }

    private string SanitizeHtml(PostMetadata.PresentationData presentation, string slidesHtml)
    {
        if (!string.IsNullOrWhiteSpace(presentation.SlidesRootUrl))
        {
            slidesHtml = slidesHtml.Replace("background-image:url(&quot;", $"background-image:url(&quot;{presentation.SlidesRootUrl}/");
            slidesHtml = slidesHtml.Replace("<img src=\"presentation-images/", $"<img src=\"{presentation.SlidesRootUrl}/presentation-images/");
        }

        return slidesHtml;
    }
}
