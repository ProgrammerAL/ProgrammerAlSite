using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.Pages;

public partial class Posts : ComponentBase
{
    private static readonly string[] PostTypes = new string[] { "Blog", "Meetup", "Conference", "Podcast", "Recording" };

    [Inject, NotNull]
    private PostSummariesProvider? PostSummariesProvider { get; set; }

    [Inject, NotNull]
    private DraftSummariesProvider? DraftSummariesProvider { get; set; }

    [Inject, NotNull]
    private TagLinksDataProvider? TagLinksDataProvider { get; set; }

    [Inject, NotNull]
    private NavigationManager? NavManager { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "tagSelections")]
    public string? QueryStringTagSelections { get; set; }

    private ImmutableArray<PostSummary> Summaries { get; set; } = ImmutableArray<PostSummary>.Empty;
    private TagLinks? TagLinks { get; set; }
    private ImmutableArray<KeyValuePair<string, bool>> TypesTagSelections { get; set; } = [];
    private ImmutableArray<KeyValuePair<string, bool>> TagSelections { get; set; } = [];

    private bool IsViewingTags { get; set; }
    private bool IsViewingDrafts => NavManager.Uri.Contains("/drafts", StringComparison.OrdinalIgnoreCase);
    private string LinkType => IsViewingDrafts ? "/drafts" : "/posts";

    protected override async Task OnInitializedAsync()
    {
        Task<ImmutableArray<PostSummary>> summariesTask;

        if (IsViewingDrafts)
        {
            summariesTask = DraftSummariesProvider.GetDraftSummariesAsync();
        }
        else
        {
            summariesTask = PostSummariesProvider.GetPostSummariesAsync();
        }

        var tagLinksTask = TagLinksDataProvider.GetTagLinksAsync();

        await Task.WhenAll(summariesTask, tagLinksTask);

        Summaries = summariesTask.Result;
        TagLinks = tagLinksTask.Result;

        RefreshTagSelections();

        IsViewingTags = !string.IsNullOrWhiteSpace(QueryStringTagSelections);

        StateHasChanged();

        await base.OnInitializedAsync();
    }

    private async Task HandleTagSelectionChangedAsync(string tag)
    {
        //Remove tag if it's already in the list, otherwise add it
        string[] tagSelectionItems;
        if (string.IsNullOrWhiteSpace(QueryStringTagSelections))
        {
            tagSelectionItems = new string[0];
        }
        else
        {
            tagSelectionItems = QueryStringTagSelections.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        if (tagSelectionItems.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            tagSelectionItems = tagSelectionItems.Where(x => !string.Equals(x, tag, StringComparison.OrdinalIgnoreCase)).Distinct().ToArray();
        }
        else
        {
            tagSelectionItems = tagSelectionItems.Append(tag).Distinct().ToArray();
        }

        QueryStringTagSelections = string.Join(",", tagSelectionItems);
        RefreshTagSelections();

        //Navigate back to this page with the new query string. Doesn't do a page refresh, UI update only
        var uri = $"/{LinkType}?tagSelections={QueryStringTagSelections}";

        NavManager.NavigateTo(uri, forceLoad: false);

        await InvokeAsync(StateHasChanged);
    }

    private void RefreshTagSelections()
    {
        if (TagLinks?.Links is object)
        {
            if (string.IsNullOrWhiteSpace(QueryStringTagSelections))
            {
                TypesTagSelections = PostTypes.OrderBy(x => x).Select(x => new KeyValuePair<string, bool>(x, false)).ToImmutableArray();
                TagSelections = TagLinks.Links
                    .Where(x => PostTypes.All(postType => !string.Equals(x.Key, postType, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.Key)
                    .Select(x => new KeyValuePair<string, bool>(x.Key, false)).ToImmutableArray();
            }
            else
            {
                var tagSelectionItems = QueryStringTagSelections?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

                TypesTagSelections = PostTypes.OrderBy(x => x).Select(x => new KeyValuePair<string, bool>(x, tagSelectionItems.Any(y => string.Equals(x, y, StringComparison.OrdinalIgnoreCase)))).ToImmutableArray();
                TagSelections = TagLinks.Links
                                    .Where(x => PostTypes.All(postType => !string.Equals(x.Key, postType, StringComparison.OrdinalIgnoreCase)))
                                    .OrderBy(x => x.Key)
                                    .Select(x => new KeyValuePair<string, bool>(x.Key, tagSelectionItems.Any(y => string.Equals(x.Key, y, StringComparison.OrdinalIgnoreCase)))).ToImmutableArray();
            }
        }
    }

    private void ToggleIsViewingTags()
    {
        IsViewingTags = !IsViewingTags;
        StateHasChanged();
    }
}
