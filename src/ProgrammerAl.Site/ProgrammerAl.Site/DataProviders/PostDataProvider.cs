using System.Text.Json;

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

    private ImmutableDictionary<string, PostData> PostDatas = new Dictionary<string, PostData>(StringComparer.OrdinalIgnoreCase).ToImmutableDictionary();

    public PostDataProvider(FileDownloader fileDownloader)
    {
        _fileDownloader = fileDownloader;
    }

    public async ValueTask<PostData> GetPostAsync(string postName)
    {
        if (PostDatas.TryGetValue(postName, out var existingValue))
        {
            return existingValue;
        }

        var pathToPost = string.Format(PostRelativeLinkTemplate, postName);
        var pathToMetadata = string.Format(PostMetadataRelativeLinkTemplate, postName);

        var htmlDownloadTask = _fileDownloader.DownloadFileTextFromSiteContentAsync(pathToPost, "text/x-markdown");
        var metadataDownloadTask = _fileDownloader.DownloadFileTextFromSiteContentAsync(pathToMetadata, "*.*");

        await Task.WhenAll(htmlDownloadTask, metadataDownloadTask);

        var metadata = JsonSerializer.Deserialize<PostMetadata>(metadataDownloadTask.Result);
        var postData = new PostData(metadata, htmlDownloadTask.Result);
        PostDatas.Add(postName, postData);

        return postData;
    }
}
