﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;
using ProgrammerAl.Site.Utilities;

namespace ProgrammerAl.Site.Pages;

public partial class Post : ComponentBase
{
    [Inject, NotNull]
    private PostDataProvider PostDataProvider { get; set; }

    [Parameter]
    public string PostUrl { get; set; }

    private MarkupString PostHtml { get; set; }
    private PostData PostData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var post = await PostDataProvider.GetPostAsync(PostUrl);

        PostHtml = new MarkupString(post.PostHtml);

        await base.OnInitializedAsync();
    }
}