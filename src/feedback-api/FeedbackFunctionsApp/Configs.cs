using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace ProgrammerAl.Site.FeedbackFunctionsApp;

public class AzureCredentialsConfig
{
    public bool? UseAzureDeveloperCliCredential { get; set; }
}

public class StorageConfig
{
    [Required, NotNull]
    public string? Endpoint { get; set; }

    [Required, NotNull]
    public string? TableName { get; set; }
}


[OptionsValidator]
public partial class StorageConfigValidateOptions : IValidateOptions<StorageConfig>
{
}