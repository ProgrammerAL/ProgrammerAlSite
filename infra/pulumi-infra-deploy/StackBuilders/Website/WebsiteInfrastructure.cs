using Pulumi;

using static ProgrammerAl.Site.IaC.StackBuilders.Website.WebsiteInfrastructure;

namespace ProgrammerAl.Site.IaC.StackBuilders.Website;

public record WebsiteInfrastructure(
    CloudflarePagesApp WebApp,
    DomainsInfrastructure DomainsInfra,
    string FullDomainEndpoint)
{
    public record CloudflarePagesApp(Pulumi.Cloudflare.PagesProject PagesProject);

    public record DomainsInfrastructure(
        Pulumi.Cloudflare.DnsRecord DomainRecord,
        Pulumi.Cloudflare.PagesDomain PagesDomain,
        Output<string> FullEndpoint);
}

