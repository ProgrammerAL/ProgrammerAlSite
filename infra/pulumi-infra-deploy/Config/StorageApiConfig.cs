using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ProgrammerAl.Site.IaC.Config;

public record StorageApiConfig(
    string CloudflareZoneId,
    string ApiDomain,
    string ServiceName)
{
}

public class StorageApiConfigDto : ConfigDtoBase<StorageApiConfig>
{
    public string? CloudflareZoneId { get; set; }
    public string? ApiDomain { get; set; }
    public string? ServiceName { get; set; }

    public override StorageApiConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(CloudflareZoneId)
            && !string.IsNullOrWhiteSpace(ServiceName)
            && !string.IsNullOrWhiteSpace(ApiDomain)
            )
        {
            return new StorageApiConfig(
                CloudflareZoneId: CloudflareZoneId,
                ServiceName: ServiceName,
                ApiDomain: ApiDomain);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
