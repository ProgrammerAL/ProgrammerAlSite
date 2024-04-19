using System.IO;
using Pulumi;
using Pulumi.Cloudflare;

using static ProgrammerAl.Site.IaC.StackBuilders.RouteFilterWorker.RouteFilterInfrastructure;
using ProgrammerAl.Site.IaC.Utilities;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;
using ProgrammerAl.Site.IaC.StackBuilders.StorageApi;

namespace ProgrammerAl.Site.IaC.StackBuilders.RouteFilterWorker;

public record RouteFilterStackBuilder(
    GlobalConfig GlobalConfig, 
    StorageApiInfrastructure StorageApiInfra)
{
    public RouteFilterInfrastructure GenerateResources()
    {
        var serviceName = GlobalConfig.RouteFilterWorkerConfig.ServiceName;
        var provider = CloudflareUtilities.GenerateCloudflareProvider($"{serviceName}-cloudflare-provider", GlobalConfig.CloudflareConfig.ApiToken);

        var workerInfra = CreateWorker(serviceName, provider);

        return new RouteFilterInfrastructure(workerInfra);
    }

    private WorkerInfrastructure CreateWorker(string serviceName, Provider provider)
    {
        var name = $"{serviceName}-script";
        var apiScript = new WorkerScript(name, new()
        {
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            Name = GlobalConfig.RouteFilterWorkerConfig.ServiceName,
            Content = File.ReadAllText(GlobalConfig.DeploymentPackagesConfig.RouteFilterWorkerFilePath),
            Module = true,
            PlainTextBindings = new[]
            {
                new Pulumi.Cloudflare.Inputs.WorkerScriptPlainTextBindingArgs
                {
                    Name = "STORAGE_API_ENDPOINT",
                    Text = StorageApiInfra.Domain.HttpsEndpoint,
                },
            }
        }, new CustomResourceOptions
        {
            Provider = provider
        });

        var workerRoute = new WorkerRoute($"{serviceName}-route", new WorkerRouteArgs
        {
            ZoneId = GlobalConfig.RouteFilterWorkerConfig.CloudflareZoneId,
            ScriptName = apiScript.Name,
            Pattern = GlobalConfig.RouteFilterWorkerConfig.RoutePattern,

        }, new CustomResourceOptions
        {
            Provider = provider
        });

        return new WorkerInfrastructure(apiScript, workerRoute);
    }
}
