using Pulumi;

using System;
using System.Threading.Tasks;
using System.Linq;

using System.Collections.Generic;
using ProgrammerAl.Site.IaC.StackBuilders.Website;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;
using ProgrammerAl.Site.IaC.StackBuilders.AncillaryApis.StorageApi;

return await Pulumi.Deployment.RunAsync(async () =>
{
    var config = new Config();
    var globalConfig = await GlobalConfig.LoadAsync(config);

    var storageApiBuilder = new StorageApiStackBuilder(globalConfig);
    var storageApiInfra = storageApiBuilder.GenerateResources();

    var websiteBuilder = new WebsiteStackBuilder(globalConfig, storageApiInfra);
    var websiteInfra = websiteBuilder.GenerateResources();

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
        
        { "WebsiteDomainEndpoint", websiteInfra.FullDomainEndpoint },

        { "ServiceVersion", Output.Create(globalConfig.ServiceConfig.ServiceVersion) },
        { "Readme", Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md")) },
    };
}
