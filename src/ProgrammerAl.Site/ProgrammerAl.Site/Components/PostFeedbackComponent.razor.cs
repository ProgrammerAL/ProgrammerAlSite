using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.HttpClients.FeedbackApi;

namespace ProgrammerAl.Site.Components;
public partial class PostFeedbackComponent : ComponentBase
{
    [Inject, NotNull]
    private IFeedbackApiHttpClient? FeedbackHttpClient { get; set; }

    [Parameter, EditorRequired]
    public string PostName { get; set; } = "";

    private string Comments { get; set; } = "";
    private SubmitStateType SubmitState { get; set; } = SubmitStateType.NotSubmitted;
    private string ErrorResultMessage { get; set; } = "";

    private async Task PostCommentsAsync()
    {
        if (string.IsNullOrWhiteSpace(PostName))
        {
            return;
        }

        SubmitState = SubmitStateType.Submitting;
        await InvokeAsync(StateHasChanged);

        var result = await FeedbackHttpClient.StoreCommentsAsync(postName: PostName, comments: Comments);
        if (result.IsValid)
        {
            SubmitState = SubmitStateType.SubmitSuccessful;
        }
        else
        {
            ErrorResultMessage = $"Error submitting feedback: {result.ResponseBody}";
            SubmitState = SubmitStateType.SubmitFailed;
        }

        await InvokeAsync(StateHasChanged);
    }

    public enum SubmitStateType
    {
        NotSubmitted,
        Submitting,
        SubmitSuccessful,
        SubmitFailed
    }
}
