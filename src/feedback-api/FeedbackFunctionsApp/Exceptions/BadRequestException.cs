using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.FeedbackFunctionsApp.Exceptions;

public class BadRequestException : HttpRequestException
{
    public BadRequestException(
        string? message) : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}
