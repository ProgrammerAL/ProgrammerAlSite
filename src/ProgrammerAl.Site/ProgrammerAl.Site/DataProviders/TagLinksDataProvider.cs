using System.Text.Json;

using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DataProviders;

public class TagLinksDataProvider
{
    private readonly FileDownloader _fileDownloader;

    private TagLinks _tagLinks = null;

    public TagLinksDataProvider(FileDownloader fileDownloader)
    {
        _fileDownloader = fileDownloader;
    }

    public async Task<TagLinks> GetTagLinksAsync()
    {
        if (_tagLinks is object)
        {
            return _tagLinks;
        }

        var tagLinksContent = await _fileDownloader.DownloadFileFromSiteContentAsync(TagLinks.TagLinksFile, "*/*");
        _tagLinks = await JsonSerializer.DeserializeAsync<TagLinks>(tagLinksContent);

        return _tagLinks;
    }
}
