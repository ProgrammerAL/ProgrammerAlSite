using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.Config;
using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.Pages;

public partial class Comics : ComponentBase
{
    [Inject, NotNull]
    private PostSummariesProvider? PostSummariesProvider { get; set; }

    [Inject, NotNull]
    private NavigationManager? NavManager { get; set; }

    [Inject, NotNull]
    private ApiConfig? ApiConfig { get; set; }

    [Parameter]
    public string? PostUrl { get; set; }

    private PostSummary? CurrentPostSummary { get; set; }
    private PostSummary? NextPostSummary { get; set; }
    private PostSummary? PreviousPostSummary { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var postSummaries = await PostSummariesProvider.GetPostSummariesAsync();

        var orderedComicSummaries = postSummaries
                                    .Where(x => !string.IsNullOrWhiteSpace(x.ComicImageLink))
                                    .OrderByDescending(p => p.PostNumber)
                                    .ToImmutableArray();

        if (string.Equals("latest", PostUrl, StringComparison.OrdinalIgnoreCase))
        {
            CurrentPostSummary = orderedComicSummaries.FirstOrDefault();
            NextPostSummary = orderedComicSummaries.Skip(1).FirstOrDefault();
        }
        else
        {
            CurrentPostSummary = postSummaries.FirstOrDefault(p => p.TitleLink == PostUrl);
            if (CurrentPostSummary is object)
            {
                var postIndex = orderedComicSummaries.IndexOf(CurrentPostSummary);
                if (postIndex < orderedComicSummaries.Length - 1)
                {
                    NextPostSummary = orderedComicSummaries.Skip(postIndex).FirstOrDefault();
                }

                if (postIndex > 0)
                {
                    PreviousPostSummary = orderedComicSummaries[postIndex - 1];
                }
            }
        }

        if (CurrentPostSummary is null)
        {
            NavigateToComic("latest");
        }

        NextPostSummary = CurrentPostSummary;
        PreviousPostSummary = CurrentPostSummary;

        await base.OnInitializedAsync();
    }

    private void OnPreviousSummarySelected(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        if (PreviousPostSummary is object)
        {
            NavigateToComic(PreviousPostSummary.TitleLink);
        }
    }

    private void OnNextSummarySelected(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        if (NextPostSummary is object)
        {
            NavigateToComic(NextPostSummary.TitleLink);
        }
    }

    private void NavigateToComic(string comicLink)
    {
        NavManager.NavigateTo($"/comics/{comicLink}");
    }

    private void OnComicsListSelected(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        NavManager.NavigateTo($"/comics-list");
    }

    private void OnComicPostSelected(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        if (CurrentPostSummary is object)
        {
            NavManager.NavigateTo($"/posts/{CurrentPostSummary.TitleLink}");
        }
    }
}
