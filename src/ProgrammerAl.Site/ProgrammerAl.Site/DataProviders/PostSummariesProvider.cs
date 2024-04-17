﻿using System.Text.Json;

using ProgrammerAl.Site.Config;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DataProviders;

public class PostSummariesProvider
{
    private readonly FileDownloader _fileDownloader;
    private readonly ApiConfig _apiConfig;
    private readonly ISiteLogger _siteLogger;

    private ImmutableArray<PostSummary> _summaries = ImmutableArray<PostSummary>.Empty;

    public PostSummariesProvider(FileDownloader fileDownloader, ApiConfig apiConfig, ISiteLogger siteLogger)
    {
        _fileDownloader = fileDownloader;
        _apiConfig = apiConfig;
        _siteLogger = siteLogger;
    }

    public async Task<ImmutableArray<PostSummary>> GetPostSummariesAsync()
    {
        if (_summaries.Any())
        {
            return _summaries;
        }

        var url = $"{_apiConfig.StorageApiBaseEndpoint}/{PostSummary.AllPostSummariesFile}".Replace("//", "/");
        var recentDataContent = await _fileDownloader.DownloadFileFromSiteContentAsync(url, "*/*");
        _summaries = await JsonSerializer.DeserializeAsync<ImmutableArray<PostSummary>>(recentDataContent);

        return _summaries;
    }
}
