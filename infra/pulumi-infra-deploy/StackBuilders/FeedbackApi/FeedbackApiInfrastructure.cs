using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

using static ProgrammerAl.Site.IaC.StackBuilders.FeedbackApi.FeedbackApiInfrastructure;

namespace ProgrammerAl.Site.IaC.StackBuilders.FeedbackApi;

public record FeedbackApiInfrastructure(
    ServiceStorageInfrastructure ServiceStorageInfra,
    FunctionsInfrastructure FeedbackApiFunctionsInfra)
{
    public record FunctionsInfrastructure(
        WebApp WebApp,
        SiteConfigArgs AppSiteConfig,
        AppServicePlan AppServicePlan,
        Output<string> HttpsEndpoint);

    public record ServiceStorageInfrastructure(
        StorageAccount StorageAccount,
        BlobContainer FunctionsDeploymentBlobContainer,
        Blob FeedbackApiFunctionsBlob,
        Table FeedbackTableStorage);
}
