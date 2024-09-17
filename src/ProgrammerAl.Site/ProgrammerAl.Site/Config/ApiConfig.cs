
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProgrammerAl.Site.Config;

public class ApiConfig
{
    [Required(AllowEmptyStrings = false), NotNull]
    public string? StorageApiBaseEndpoint { get; set; }

    [Required(AllowEmptyStrings = false), NotNull]
    public string? FeedbackApiBaseEndpoint { get; set; }

    [Required, NotNull]
    public TimeSpan? HttpTimeout { get; set; }
}
