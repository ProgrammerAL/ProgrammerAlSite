
using System.Diagnostics.CodeAnalysis;

namespace ProgrammerAl.Site.HttpClients;

public record HttpResponseResult(bool IsValid, string ResponseBody, int StatusCode);

public record HttpResponseResult<T>(T? ResponseObject, string? ErrorMessage, int StatusCode)
    where T : class
{
    public HttpResponseResult(T responseObject, int statusCode)
    : this(ResponseObject: responseObject, ErrorMessage: null, StatusCode: statusCode)
    { }

    public HttpResponseResult(string errorMessage, int statusCode)
        : this(ResponseObject: null, ErrorMessage: errorMessage, StatusCode: statusCode)
    { }

    [MemberNotNullWhen(true, nameof(ResponseObject))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsValid => ResponseObject is object;
}
