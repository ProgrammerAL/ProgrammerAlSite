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

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(PostUrl))
        {
            var post = await PostDataProvider.GetPostAsync(PostUrl);

            if (post is object)
            {
                PostHtml = new MarkupString(post.PostHtml);
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

        var feedbackStartDate = PostData.Metadata.ReleaseDate.AddDays(-10);
        var feedbackEndDate = PostData.Metadata.ReleaseDate.AddDays(4);

        var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (todayDate < feedbackStartDate
            || todayDate > feedbackEndDate)
        {
            return false;
        }

        return true;
    }
}
