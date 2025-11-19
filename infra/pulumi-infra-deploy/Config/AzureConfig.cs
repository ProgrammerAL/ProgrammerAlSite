using Pulumi;
using Pulumi.AzureNative.Authorization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Config;

public record AzureConfig(
    GetClientConfigResult ClientConfig,
    string Location,
    string ResourceGroupName);

public class AzureConfigDto : ConfigDtoBase<AzureConfig>
{
    public GetClientConfigResult? ClientConfig { get; set; }
    public string? Location { get; set; }
    public string? ResourceGroupName { get; set; }

    public override AzureConfig GenerateValidConfigObject()
    {
        if (ClientConfig != null
            && !string.IsNullOrWhiteSpace(Location)
            && !string.IsNullOrWhiteSpace(ResourceGroupName))
        {
            return new(ClientConfig, Location, ResourceGroupName);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
