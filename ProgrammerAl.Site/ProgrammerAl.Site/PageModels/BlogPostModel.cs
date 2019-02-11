using System;
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
        private HttpClient HttpClient { get; set; }

        [Inject]
        private IConfig Config { get; set; }

        [Parameter]
        private string PostUrl { get; set; }

        protected MarkupString BlogPostHtml { get; set; }

        protected override async Task OnInitAsync()
        {
            var downloader = new FileDownloader();
            var pathToBlogPost = string.Format(BlogPostRelativeLinkTemplate, PostUrl);
            var response = await downloader.DownloadFileFromSiteContentAsync(HttpClient, Config, pathToBlogPost, "text/x-markdown");

            BlogPostHtml = new MarkupString(response);

            await base.OnInitAsync();
        }
    }
}
