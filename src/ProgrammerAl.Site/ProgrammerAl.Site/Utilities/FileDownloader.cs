using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ProgrammerAl.Site.Utilities
{
    public class FileDownloader
    {
        public async Task<string> DownloadFileTextFromSiteContentAsync(IConfig config, string relativeFilePath, string responseAcceptType)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(config.SiteContentUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

            return await httpClient.GetStringAsync(relativeFilePath);
        }

        public async Task<Stream> DownloadFileFromSiteContentAsync(IConfig config, string relativeFilePath, string responseAcceptType)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(config.SiteContentUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

            return await httpClient.GetStreamAsync(relativeFilePath);
        }
    }
}
