using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ProgrammerAl.Site.IaC.Config;

public record RouteFilterWorkerConfig(
    string CloudflareZoneId,
    string RoutePattern,
    string ServiceName)
{
}

public class RouteFilterWorkerConfigDto : ConfigDtoBase<RouteFilterWorkerConfig>
{
    public string? CloudflareZoneId { get; set; }
    public string? RoutePattern { get; set; }
    public string? ServiceName { get; set; }

    public override RouteFilterWorkerConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(CloudflareZoneId)
            && !string.IsNullOrWhiteSpace(ServiceName)
            && !string.IsNullOrWhiteSpace(RoutePattern)
            )
        {
            return new RouteFilterWorkerConfig(
                CloudflareZoneId: CloudflareZoneId,
                ServiceName: ServiceName,
                RoutePattern: RoutePattern);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
