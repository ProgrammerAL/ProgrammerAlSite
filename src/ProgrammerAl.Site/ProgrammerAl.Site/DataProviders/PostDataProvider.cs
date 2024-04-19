using System.Text.Json;

using ProgrammerAl.Site.Config;
using ProgrammerAl.Site.PostDataEntities;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DataProviders;

public record PostData(PostMetadata Metadata, string PostHtml);
public class PostDataProvider
{
    private const string PostRelativeLinkTemplate = $"Posts/{{0}}/{PostEntry.HtmlFileName}";
    private const string PostMetadataRelativeLinkTemplate = $"Posts/{{0}}/{PostMetadata.FileName}";

    private readonly FileDownloader _fileDownloader;
    private readonly ApiConfig _apiConfig;
    private readonly ISiteLogger _siteLogger;

    private ImmutableDictionary<string, PostData> PostDatas = new Dictionary<string, PostData>(StringComparer.OrdinalIgnoreCase).ToImmutableDictionary();

    public PostDataProvider(FileDownloader fileDownloader, ApiConfig apiConfig, ISiteLogger siteLogger)
    {
        _fileDownloader = fileDownloader;
        _apiConfig = apiConfig;
        _siteLogger = siteLogger;
    }

    public async ValueTask<PostData?> GetPostAsync(string postName)
    {
        if (PostDatas.TryGetValue(postName, out var existingValue))
        {
            return existingValue;
        }

        var pathToPost = $"{_apiConfig.StorageApiBaseEndpoint}/{string.Format(PostRelativeLinkTemplate, postName)}";
        var pathToMetadata = $"{_apiConfig.StorageApiBaseEndpoint}/{string.Format(PostMetadataRelativeLinkTemplate, postName)}";

        var htmlDownloadTask = _fileDownloader.DownloadFileTextFromSiteContentAsync(pathToPost, "text/x-markdown");
        var metadataDownloadTask = _fileDownloader.DownloadFileTextFromSiteContentAsync(pathToMetadata, "*/*");

        await Task.WhenAll(htmlDownloadTask, metadataDownloadTask);

        var metadata = JsonSerializer.Deserialize<PostMetadata>(metadataDownloadTask.Result);
        if (metadata is null)
        {
            _siteLogger.Log($"Could not deserialize post metadata from: {pathToMetadata}");
            return null;
        }

        var postData = new PostData(metadata, htmlDownloadTask.Result);
        PostDatas.Add(postName, postData);

        return postData;
    }
}
