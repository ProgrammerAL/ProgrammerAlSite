﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.Utilities;

namespace ProgrammerAl.Site.PageModels
{
    public class BlogPostModel : ComponentBase
    {
        private const string BlogPostRelativeLinkTemplate = "BlogPosts/{0}.html";

        [Inject]
        private IConfig Config { get; set; }

        [Parameter]
        public string PostUrl { get; set; }

        protected MarkupString PostHtml { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var downloader = new FileDownloader();
            var pathToBlogPost = string.Format(BlogPostRelativeLinkTemplate, PostUrl);
            var response = await downloader.DownloadFileTextFromSiteContentAsync(Config, pathToBlogPost, "text/x-markdown");

            PostHtml = new MarkupString(response);

            await base.OnInitializedAsync();
        }
    }
}
