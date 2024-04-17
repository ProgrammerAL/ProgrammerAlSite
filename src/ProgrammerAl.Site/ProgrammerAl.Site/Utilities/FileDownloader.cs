using System;
using System.IO;
using System.Net.Http;

namespace ProgrammerAl.Site.Utilities;

public class FileDownloader
{
    private readonly IConfig _config;

    public FileDownloader(IConfig config)
    {
        _config = config;
    }

    public async Task<string> DownloadFileTextFromSiteContentAsync(string relativeFilePath, string responseAcceptType)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_config.SiteContentUrl);
        httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

        return await httpClient.GetStringAsync(relativeFilePath);
    }

    public async Task<Stream> DownloadFileFromSiteContentAsync(string relativeFilePath, string responseAcceptType)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_config.SiteContentUrl);
        httpClient.DefaultRequestHeaders.Add("Accept", responseAcceptType);

        return await httpClient.GetStreamAsync(relativeFilePath);
    }
}
