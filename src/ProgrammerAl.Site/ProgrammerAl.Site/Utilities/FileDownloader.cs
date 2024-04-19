using System;
using System.IO;
using System.Net.Http;

namespace ProgrammerAl.Site.Utilities;

public class FileDownloader
{
    public async Task<string> DownloadFileTextFromSiteContentAsync(string url, string responseAcceptType)
    {
        url = SanitizeUrl(url);
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

        return await httpClient.GetStringAsync(url);
    }

    public async Task<Stream> DownloadFileFromSiteContentAsync(string url, string responseAcceptType)
    {
        url = SanitizeUrl(url);
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

        return await httpClient.GetStreamAsync(url);
    }

    private string SanitizeUrl(string url) => url.Replace("//", "/");
}
