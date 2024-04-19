using System.Text.Json;

using ProgrammerAl.Site.Config;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DataProviders;

public class TagLinksDataProvider
{
    private readonly FileDownloader _fileDownloader;
    private readonly ApiConfig _apiConfig;
    private readonly ISiteLogger _siteLogger;

    private TagLinks? _tagLinks = null;

    public TagLinksDataProvider(FileDownloader fileDownloader, ApiConfig apiConfig, ISiteLogger siteLogger)
    {
        _fileDownloader = fileDownloader;
        _apiConfig = apiConfig;
        _siteLogger = siteLogger;
    }

    public async Task<TagLinks?> GetTagLinksAsync()
    {
        if (_tagLinks is object)
        {
            return _tagLinks;
        }

        var url = $"{_apiConfig.StorageApiBaseEndpoint}/{TagLinks.TagLinksFile}";
        var tagLinksContent = await _fileDownloader.DownloadFileFromSiteContentAsync(url, "*/*");
        _tagLinks = await JsonSerializer.DeserializeAsync<TagLinks>(tagLinksContent);

        return _tagLinks;
    }
}
