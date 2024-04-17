using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.Pages;

public partial class ComicsList : ComponentBase
{
    [Inject]
    private PostSummariesProvider PostSummariesProvider { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    [Inject]
    private IConfig Config { get; set; }

    private ImmutableArray<PostSummary> ComicSummaries { get; set; } = ImmutableArray<PostSummary>.Empty;

    protected override async Task OnInitializedAsync()
    {
        var postSummaries = await PostSummariesProvider.GetPostSummariesAsync();

        ComicSummaries = postSummaries
                                    .Where(x => !string.IsNullOrWhiteSpace(x.ComicImageLink))
                                    .OrderByDescending(p => p.PostNumber)
                                    .ToImmutableArray();


        await base.OnInitializedAsync();
    }

    private void NavigateToComic(string comicLink)
    {
        NavManager.NavigateTo($"/comics/{comicLink}");
    }
}
