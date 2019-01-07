using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.Utilities
{
    public class FileDownloader
    {
        public async Task<string> DownloadFileFromSiteContentAsync(HttpClient httpClient, IConfig config, string relativeFilePath, string responseAcceptType)
        {

            httpClient.BaseAddress = new Uri(config.SiteContentUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

            //https://developersidequestssite.blob.core.windows.net/sitecontent/RecentData.json
            return await httpClient.GetStringAsync(relativeFilePath);
        }
    }
}
