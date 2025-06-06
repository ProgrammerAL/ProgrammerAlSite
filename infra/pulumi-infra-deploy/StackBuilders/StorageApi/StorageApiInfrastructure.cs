﻿using Pulumi;

using static ProgrammerAl.Site.IaC.StackBuilders.StorageApi.StorageApiInfrastructure;

using Cloudflare = Pulumi.Cloudflare;

namespace ProgrammerAl.Site.IaC.StackBuilders.StorageApi;

public record StorageApiInfrastructure(
    StorageInfrastructure StorageInfra,
    ApiInfrastructure ApiInfra,
    DomainInfrastructure Domain)
{

    public record StorageInfrastructure(
        Cloudflare.R2Bucket Bucket);

    public record ApiInfrastructure(
        Cloudflare.WorkersScript Script,
        Pulumi.Random.RandomUuid AdminAuthToken);

    public record DomainInfrastructure(
        Cloudflare.WorkersCustomDomain WorkerDomain,
        Output<string> HttpsEndpoint);
}
