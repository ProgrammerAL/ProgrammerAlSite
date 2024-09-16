using System.Net;
using System.Text;

using Azure.Data.Tables;

using ProgrammerAl.Site.FeedbackApi.AzureUtils;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace ProgrammerAl.Site.FeedbackApi.Functions.StoreComment;

public class StoreCommentsFunction
{
    private readonly IOptions<StorageConfig> _storageConfig;
    private readonly IAzureCredentialsManager _credentialsManager;

    public StoreCommentsFunction(IOptions<StorageConfig> storageConfig, IAzureCredentialsManager credentialsManager)
    {
        _storageConfig = storageConfig;
        _credentialsManager = credentialsManager;
    }

    [Function("store-comments")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var requestDto = await req.ReadFromJsonAsync<StoreCommentsRequestDto>();
        if (requestDto is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var requestObject = requestDto.GenerateValidObject();
        var itemKey = Guid.NewGuid().ToString();
        var partitionKey = requestObject.PostName.ToLower();

        var tableUri = new Uri(_storageConfig.Value.Endpoint);
        var tableClient = new TableClient(tableUri, _storageConfig.Value.TableName, _credentialsManager.AzureTokenCredential);

        await tableClient.CreateIfNotExistsAsync();

        // Make a dictionary entity by defining a <see cref="TableEntity">.
        var tableEntity = new TableEntity(partitionKey: partitionKey, rowKey: itemKey)
        {
            { "Post", requestObject.PostName },
            { "Comments", requestObject.Comments },
        };

        await tableClient.AddEntityAsync(tableEntity);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
