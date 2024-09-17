
using System.Diagnostics.CodeAnalysis;

namespace PurpleSpikeProductions.ArcadeServices.Public.WebClient.HttpClients.Responses;

public abstract class HttpResponseDtoBase<T> where T : class
{
    public abstract bool TryGenerateValidResponseObject([NotNullWhen(true)] out T? outResponse);
}
