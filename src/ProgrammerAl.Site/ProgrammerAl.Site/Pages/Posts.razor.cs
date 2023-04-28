using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.Pages;

public partial class Posts : ComponentBase
{
    [Inject]
    private IConfig Config { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "tagSelections")]
    public string QueryStringTagSelections { get; set; }

    private BlogPostSummary[] BlogPosts { get; set; }
    private TagLinks TagLinks { get; set; }
    private ImmutableArray<KeyValuePair<string, bool>> TagSelections { get; set; } = ImmutableArray.Create<KeyValuePair<string, bool>>();

    private bool IsViewingTags { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var downloader = new FileDownloader();

        var recentDataContentTask = downloader.DownloadFileFromSiteContentAsync(Config, "BlogPosts.json", "*/*");
        var tagLinksTask = downloader.DownloadFileFromSiteContentAsync(Config, "TagLinks.json", "*/*");

        _ = await Task.WhenAll(recentDataContentTask, tagLinksTask);

        var recentDataContent = recentDataContentTask.Result;
        var tagLinksContent = tagLinksTask.Result;

        BlogPosts = await JsonSerializer.DeserializeAsync<BlogPostSummary[]>(recentDataContent);
        TagLinks = await JsonSerializer.DeserializeAsync<TagLinks>(tagLinksContent);

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
            tagSelectionItems = QueryStringTagSelections?.Split(',', StringSplitOptions.RemoveEmptyEntries);
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
        var uri = $"/posts?tagSelections={QueryStringTagSelections}";
        NavManager.NavigateTo(uri, forceLoad: false);

        await InvokeAsync(StateHasChanged);
    }

    private void RefreshTagSelections()
    {
        if (string.IsNullOrWhiteSpace(QueryStringTagSelections))
        {
            TagSelections = TagLinks.Links.OrderBy(x => x.Key).Select(x => new KeyValuePair<string, bool>(x.Key, false)).ToImmutableArray();
        }
        else
        {
            var tagSelectionItems = QueryStringTagSelections?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            TagSelections = TagLinks.Links.OrderBy(x => x.Key).Select(x => new KeyValuePair<string, bool>(x.Key, tagSelectionItems.Any(y => string.Equals(x.Key, y, StringComparison.OrdinalIgnoreCase)))).ToImmutableArray();
        }
    }

    private void ToggleIsViewingTags()
    {
        IsViewingTags = !IsViewingTags;
        StateHasChanged();
    }
}
