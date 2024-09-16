
using ProgrammerAl.Site.FeedbackFunctionsApp.Exceptions;

namespace ProgrammerAl.Site.FeedbackFunctionsApp.Functions;

public record StoreCommentsRequest(string PostName, string Comments);

public class StoreCommentsRequestDto
{
    public string? PostName { get; set; }
    public string? Comments { get; set; }

    public StoreCommentsRequest GenerateValidObject()
    {
        if (string.IsNullOrWhiteSpace(PostName))
        {
            throw new BadRequestException("Request missing property: PostName");
        }
        else if (string.IsNullOrWhiteSpace(Comments))
        {
            throw new BadRequestException("Request missing property: Comments");
        }

        return new StoreCommentsRequest(PostName, Comments);
    }
}
