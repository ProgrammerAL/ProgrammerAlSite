using System.Text.Json;

using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DataProviders;

public class PostSummariesProvider
{
    private readonly FileDownloader _fileDownloader;

    private ImmutableArray<PostSummary> _summaries = ImmutableArray<PostSummary>.Empty;

    public PostSummariesProvider(FileDownloader fileDownloader)
    {
        _fileDownloader = fileDownloader;
    }

    public async Task<ImmutableArray<PostSummary>> GetPostSummariesAsync()
    {
        if (_summaries.Any())
        {
            return _summaries;
        }

        var recentDataContent = await _fileDownloader.DownloadFileFromSiteContentAsync(PostSummary.AllPostSummariesFile, "*/*");
        _summaries = await JsonSerializer.DeserializeAsync<ImmutableArray<PostSummary>>(recentDataContent);

        return _summaries;
    }
}
