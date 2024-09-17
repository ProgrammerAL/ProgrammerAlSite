using System.Net.Http;

namespace ProgrammerAl.Site.HttpClients.FeedbackApi;

public interface IFeedbackApiHttpClient
{
    ValueTask<HttpResponseResult> StoreCommentsAsync(string postName, string comments);
}

public class FeedbackApiHttpClient : HttpClientBase, IFeedbackApiHttpClient
{
    public FeedbackApiHttpClient(HttpClient client)
        : base(client)
    {
    }

    public async ValueTask<HttpResponseResult> StoreCommentsAsync(string postName, string comments)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "store-comments");
        var requestDto = new StoreCommentsRequest(postName, comments);
        return await RunAsync(request, requestDto);
    }
}
