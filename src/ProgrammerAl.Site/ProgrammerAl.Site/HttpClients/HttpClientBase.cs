using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;

using PurpleSpikeProductions.ArcadeServices.Public.WebClient.HttpClients.Responses;

namespace ProgrammerAl.Site.HttpClients;

public abstract class HttpClientBase
{
    private readonly HttpClient _client;

    protected HttpClientBase(HttpClient client)
    {
        _client = client;
    }

    protected async ValueTask<HttpResponseResult<TResponse>> RunAsync<TRequest, TResponseDto, TResponse>(HttpRequestMessage requestMessage, TRequest requestBody)
        where TResponse : class
        where TResponseDto : HttpResponseDtoBase<TResponse>
    {
        var requestJson = JsonSerializer.Serialize(requestBody);
        requestMessage.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        return await RunAsync<TResponseDto, TResponse>(requestMessage);
    }

    protected async ValueTask<HttpResponseResult<TResponse>> RunAsync<TResponseDto, TResponse>(HttpRequestMessage requestMessage)
        where TResponse : class
        where TResponseDto : HttpResponseDtoBase<TResponse>
    {
        var response = await _client.SendAsync(requestMessage);

        var bodyContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponseResult<TResponse>(errorMessage: bodyContent, (int)response.StatusCode);
        }

        var deserializedDto = JsonSerializer.Deserialize<TResponseDto>(bodyContent);

        if (deserializedDto is null)
        {
            return new HttpResponseResult<TResponse>(errorMessage: bodyContent, (int)response.StatusCode);
        }

        if (!deserializedDto.TryGenerateValidResponseObject(out TResponse? responseRecord))
        {
            return new HttpResponseResult<TResponse>(errorMessage: bodyContent, (int)response.StatusCode);
        }

        return new HttpResponseResult<TResponse>(responseObject: responseRecord, (int)response.StatusCode);
    }

    protected async ValueTask<HttpResponseResult> RunAsync<TRequest>(HttpRequestMessage requestMessage, TRequest requestBody)
    {
        var requestJson = JsonSerializer.Serialize(requestBody);
        requestMessage.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(requestMessage);
        var bodyContent = await response.Content.ReadAsStringAsync();

        return new HttpResponseResult(IsValid: response.IsSuccessStatusCode, ResponseBody: bodyContent, StatusCode: (int)response.StatusCode);
    }

    protected async ValueTask<HttpResponseResult> RunAsync(HttpRequestMessage requestMessage)
    {
        var response = await _client.SendAsync(requestMessage);
        var bodyContent = await response.Content.ReadAsStringAsync();

        return new HttpResponseResult(IsValid: response.IsSuccessStatusCode, ResponseBody: bodyContent, StatusCode: (int)response.StatusCode);
    }
}
