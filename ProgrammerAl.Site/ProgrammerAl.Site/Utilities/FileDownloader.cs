using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ProgrammerAl.Site.Utilities
{
    public class FileDownloader
    {
        //public async Task<T> DownloadFileFromSiteContentAsync<T>(HttpClient httpClient, IConfig config, string relativeFilePath, string responseAcceptType)
        //{
        //    httpClient.BaseAddress = new Uri(config.SiteContentUrl);
        //    httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

        //    //https://developersidequestssite.blob.core.windows.net/sitecontent/RecentData.json
        //    var jsonString = await httpClient.GetStringAsync(relativeFilePath);

        //    return await httpClient.GetJsonAsync<T>(relativeFilePath);
        //}

        public async Task<string> DownloadFileFromSiteContentAsync(HttpClient httpClient, IConfig config, string relativeFilePath, string responseAcceptType)
        {
            httpClient.BaseAddress = new Uri(config.SiteContentUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

            //https://developersidequestssite.blob.core.windows.net/sitecontent/RecentData.json
            return await httpClient.GetStringAsync(relativeFilePath);
        }
    }
}
