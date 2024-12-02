using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;

namespace ProgrammerAl.Site.Pages;

public partial class Post : ComponentBase
{
    [Inject, NotNull]
    private PostDataProvider? PostDataProvider { get; set; }

    [Parameter]
    public string? PostUrl { get; set; }

    private MarkupString PostHtml { get; set; }
    private PostData? PostData { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        PostData = null;
        PostHtml = new MarkupString(string.Empty);

        if (!string.IsNullOrWhiteSpace(PostUrl))
        {
            PostData = await PostDataProvider.GetPostAsync(PostUrl);

            if (PostData is object)
            {
                PostHtml = new MarkupString(PostData.PostHtml);
            }
        }

        await InvokeAsync(StateHasChanged);

        await base.OnParametersSetAsync();
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
