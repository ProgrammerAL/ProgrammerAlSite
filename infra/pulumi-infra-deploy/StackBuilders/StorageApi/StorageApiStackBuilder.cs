using System.IO;
using Pulumi;
using Pulumi.Cloudflare;
using Cloudflare = Pulumi.Cloudflare;

using static ProgrammerAl.Site.IaC.StackBuilders.StorageApi.StorageApiInfrastructure;
using ProgrammerAl.Site.IaC.Utilities;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;

namespace ProgrammerAl.Site.IaC.StackBuilders.StorageApi;

public record StorageApiStackBuilder(
    GlobalConfig GlobalConfig)
{
    public StorageApiInfrastructure GenerateResources()
    {
        var serviceName = GlobalConfig.StorageApiConfig.ServiceName;
        var provider = CloudflareUtilities.GenerateCloudflareProvider($"{serviceName}-cloudflare-provider", GlobalConfig.CloudflareConfig.ApiToken);

        var storageInfra = CreateStorage(serviceName, provider);
        var apiInfra = CreateApi(storageInfra, serviceName, provider);
        var domainInfra = GenerateDomainEntries(apiInfra, serviceName, provider);

        return new StorageApiInfrastructure(storageInfra, apiInfra, domainInfra);
    }

    private StorageInfrastructure CreateStorage(string serviceName, Provider provider)
    {
        var bucketName = $"{serviceName}-bucket";
        var bucket = new R2Bucket(bucketName, new()
        {
            Name = bucketName,
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            //Disable location hint for now because there's a bug: https://github.com/pulumi/pulumi-cloudflare/issues/465
            // Honestly not sure if we even need this, but leaving the code in for now in case we want it in the future
            //Location = GlobalConfig.CloudflareStorageConfig.LocationHint,
        }, new CustomResourceOptions
        {
            Provider = provider,
        });

        return new StorageInfrastructure(bucket);
    }
    private ApiInfrastructure CreateApi(StorageInfrastructure storageInfra, string serviceName, Provider provider)
    {
        var adminAuthToken = new Pulumi.Random.RandomUuid($"{serviceName}-admin-auth-token", new Pulumi.Random.RandomUuidArgs
        {
        });

        Pulumi.Log.Info($"Using storage api js file from: {GlobalConfig.DeploymentPackagesConfig.StorageApiWorkerFilePath}");

        var name = $"{serviceName}-script";
        var apiScript = new WorkerScript(name, new()
        {
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            Name = GlobalConfig.StorageApiConfig.ServiceName,
            Content = File.ReadAllText(GlobalConfig.DeploymentPackagesConfig.StorageApiWorkerFilePath),
            Module = true,
            R2BucketBindings = new[]
            {
                new Cloudflare.Inputs.WorkerScriptR2BucketBindingArgs
                {
                    Name = "STORAGE_BUCKET",
                    BucketName = storageInfra.Bucket.Name,
                },
            },
            SecretTextBindings = new[]
            {
                new Cloudflare.Inputs.WorkerScriptSecretTextBindingArgs
                {
                    Name = "ADMIN_TOKEN",
                    Text = adminAuthToken.Result,
                },
            },
        }, new CustomResourceOptions
        {
            Provider = provider
        });

        return new ApiInfrastructure(apiScript, adminAuthToken);
    }

    private DomainInfrastructure GenerateDomainEntries(ApiInfrastructure apiInfra, string serviceName, Provider provider)
    {
        var domainEndpoint = $"{GlobalConfig.StorageApiConfig.ServiceName}.{GlobalConfig.StorageApiConfig.ApiDomain}".ToLower();

        var workerDomain = new WorkerDomain($"{serviceName}-worker-domain", new()
        {
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            ZoneId = GlobalConfig.StorageApiConfig.CloudflareZoneId,
            Service = apiInfra.Script.Name,
            Hostname = domainEndpoint
        }, new CustomResourceOptions
        {
            Provider = provider
        });

        var httpsEndpoint = Output.Create($"https://{domainEndpoint}");
        return new(workerDomain, httpsEndpoint);
    }
}
