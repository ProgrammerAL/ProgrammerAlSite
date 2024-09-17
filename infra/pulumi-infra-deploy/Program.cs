using Pulumi;

using System;
using System.Threading.Tasks;
using System.Linq;

using System.Collections.Generic;
using ProgrammerAl.Site.IaC.StackBuilders.Website;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;
using ProgrammerAl.Site.IaC.StackBuilders.StorageApi;
using ProgrammerAl.Site.IaC.StackBuilders.RouteFilterWorker;

return await Pulumi.Deployment.RunAsync(async () =>
{
    var clientConfig = await Pulumi.AzureNative.Authorization.GetClientConfig.InvokeAsync();

    var config = new Config();
    var globalConfig = await GlobalConfig.LoadAsync(config);

    var storageApiBuilder = new StorageApiStackBuilder(globalConfig);
    var storageApiInfra = storageApiBuilder.GenerateResources();

    var websiteBuilder = new WebsiteStackBuilder(globalConfig, storageApiInfra);
    var websiteInfra = websiteBuilder.GenerateResources();

    var routeFilterWorkerBuilder = new RouteFilterStackBuilder(globalConfig, storageApiInfra);
    var routeFilterWorkerInfra = routeFilterWorkerBuilder.GenerateResources();

    var azureRgBuilder = new AzureResourceGroupStackBuilder(globalConfig);
    var azureRgInfra = azureRgBuilder.GenerateResources();

    var feedbackFunctionsBuilder = new FeedbackApiStackBuilder(globalConfig, azureRgInfra, clientConfig);
    var feedbackFunctionsInfra = feedbackFunctionsBuilder.GenerateResources();

    return GenerateOutputs(websiteInfra, storageApiInfra, globalConfig);
});

static Dictionary<string, object?> GenerateOutputs(
    WebsiteInfrastructure websiteInfra,
    StorageApiInfrastructure storageApiInfra,
    GlobalConfig globalConfig)
{
    return new Dictionary<string, object?>
    {
        { "StorageApiHttpsEndpoint", storageApiInfra.Domain.HttpsEndpoint },
        { "StorageApiAdminAuthToken", Output.CreateSecret(storageApiInfra.ApiInfra.AdminAuthToken.Result) },

        { "WebsiteDomainEndpoint", websiteInfra.FullDomainEndpoint },

        { "Readme", Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md")) },
    };
}
