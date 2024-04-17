
using System;


namespace ProgrammerAl.Site.IaC.Config;

public record WebClientInfrastructureConfig(
    string RootDomain,
    string Subdomain,
    string CloudflareZoneId)
{

    public string DomainEndpoint = $"{Subdomain}.{RootDomain}";
    public string HttpsDomainEndpoint = $"https://{Subdomain}.{RootDomain}";
    public string ResourceName => "web-client";
    public string StorageAccountName => "webclient";
}

public class WebClientInfrastructureConfigDto : ConfigDtoBase<WebClientInfrastructureConfig>
{
    public string? RootDomain { get; set; }
    public string? Subdomain { get; set; }
    public string? CloudflareZoneId { get; set; }

    public override WebClientInfrastructureConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(RootDomain)
            && !string.IsNullOrWhiteSpace(Subdomain)
            && !string.IsNullOrWhiteSpace(CloudflareZoneId)
            )
        {
            return new WebClientInfrastructureConfig(
                RootDomain,
                Subdomain,
                CloudflareZoneId
                );
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
