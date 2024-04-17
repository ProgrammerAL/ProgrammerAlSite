using Pulumi;

using static ProgrammerAl.Site.IaC.StackBuilders.AncillaryApis.StorageApi.StorageApiInfrastructure;

using Cloudflare = Pulumi.Cloudflare;

namespace ProgrammerAl.Site.IaC.StackBuilders.AncillaryApis.StorageApi;

public record StorageApiInfrastructure(
    StorageInfrastructure StorageInfra,
    ApiInfrastructure ApiInfra,
    DomainInfrastructure Domain)
{

    public record StorageInfrastructure(
        Cloudflare.R2Bucket Bucket);

    public record ApiInfrastructure(
        Cloudflare.WorkerScript Script,
        Pulumi.Random.RandomUuid AdminAuthToken);

    public record DomainInfrastructure(
        Cloudflare.WorkerDomain WorkerDomain,
        Output<string> HttpsEndpoint);
}
