using Pulumi;
using System;
using System.Threading.Tasks;
using System.Linq;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi.AzureNative.Authorization;

using ProgrammerAl.Site.IaC.StackBuilders.FeedbackApi;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;

using AzureNative = Pulumi.AzureNative;
using ProgrammerAl.Site.IaC.StackBuilders.AzureResourceGroup;

using static ProgrammerAl.Site.IaC.StackBuilders.FeedbackApi.FeedbackApiInfrastructure;
using ProgrammerAl.Site.IaC.Utilities;

public record FeedbackApiStackBuilder(GlobalConfig GlobalConfig,
    AzureResourceGroupInfrasatructure AzureResourceGroupInfra,
    GetClientConfigResult ClientConfig)
{
    private Output<string> ResourceGroupName => AzureResourceGroupInfra.ResourceGroupInfra.ResourceGroup.Name;

    public FeedbackApiInfrastructure GenerateResources()
    {
        var storageInfra = GenerateStorageInfrastructure();

        var publicFunctionsInfra = GeneratePublicApiAzureFunctionsInfrastructure(
            storageInfra);

        return new FeedbackApiInfrastructure(storageInfra, publicFunctionsInfra);
    }

    private ServiceStorageInfrastructure GenerateStorageInfrastructure()
    {
        var storageAccount = new StorageAccount("feedbackapistg",
            new StorageAccountArgs
            {
                ResourceGroupName = ResourceGroupName,
                Sku = new AzureNative.Storage.Inputs.SkuArgs { Name = AzureNative.Storage.SkuName.Standard_GRS, },
                Kind = Kind.StorageV2,
                EnableHttpsTrafficOnly = true,
                MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
                AccessTier = AccessTier.Hot,
                AllowSharedKeyAccess = true,
                SasPolicy = new AzureNative.Storage.Inputs.SasPolicyArgs
                {
                    ExpirationAction = ExpirationAction.Log,
                    SasExpirationPeriod = "00.01:00:00"
                }
            });

        _ = new BlobServiceProperties("feedback-function-app-blob-properties", new BlobServicePropertiesArgs
        {
            IsVersioningEnabled = false,
            BlobServicesName = "default",
            AccountName = storageAccount.Name,
            ResourceGroupName = ResourceGroupName,
            Cors = new AzureNative.Storage.Inputs.CorsRulesArgs
            {
                CorsRules = new[]
                {
                    new AzureNative.Storage.Inputs.CorsRuleArgs
                    {
                        AllowedHeaders = new[] { "*" },
                        AllowedMethods = new InputList<Union<string, Pulumi.AzureNative.Storage.AllowedMethods>>()
                        {
                            AllowedMethods.DELETE,
                            AllowedMethods.GET,
                            AllowedMethods.HEAD,
                            AllowedMethods.MERGE,
                            AllowedMethods.POST,
                            AllowedMethods.OPTIONS,
                            AllowedMethods.PUT
                        },
                        AllowedOrigins = new[] { "*" },
                        ExposedHeaders = new[] { "*" },
                        MaxAgeInSeconds = 100,
                    }
                }
            }
        });

        var feedbackTableStorage = new Table("feedback-table-storage",
            new TableArgs
            {
                AccountName = storageAccount.Name,
                ResourceGroupName = ResourceGroupName,
                TableName = "FeedbackTable",
            });

        //Storage Container to host the Azure Functions
        var functionsDeploymentContainer = new BlobContainer("feedback-api-function-app-blobs",
            new BlobContainerArgs
            {
                AccountName = storageAccount.Name,
                PublicAccess = PublicAccess.None,
                ResourceGroupName = ResourceGroupName,
            });

        var feedbackFunctionsBlob = new Blob("feedback-api-function-app-zip-blob",
            new BlobArgs
            {
                AccountName = storageAccount.Name,
                ContainerName = functionsDeploymentContainer.Name,
                AccessTier = BlobAccessTier.Hot,
                ResourceGroupName = ResourceGroupName,
                Source = new FileArchive(GlobalConfig.DeploymentPackagesConfig.FeedbackApiFunctionsZipFilePath)
            });

        return new ServiceStorageInfrastructure(
            storageAccount,
            functionsDeploymentContainer,
            feedbackFunctionsBlob,
            feedbackTableStorage);
    }

    private FunctionsInfrastructure GeneratePublicApiAzureFunctionsInfrastructure(
        ServiceStorageInfrastructure storageInfra)
    {
        //Create the App Service Plan
        var appServicePlan = new AppServicePlan("feedback-function-app-service-plan",
            new AppServicePlanArgs
            {
                ResourceGroupName = ResourceGroupName,
                Kind = "Linux",
                Sku = new SkuDescriptionArgs { Tier = "Dynamic", Name = "Y1" },
                // For Linux, you need to change the plan to have Reserved = true property.
                Reserved = true,
            });

        var functionAppSiteConfig = new SiteConfigArgs
        {
            LinuxFxVersion = "DOTNET-ISOLATED|8.0",
            AppSettings = new[]
            {
                new NameValuePairArgs
                {
                    Name = "AzureWebJobsStorage__accountname",
                    Value = storageInfra.StorageAccount.Name
                },
                new NameValuePairArgs
                {
                    Name = "WEBSITE_RUN_FROM_PACKAGE",
                    Value = storageInfra.FeedbackApiFunctionsBlob.Url
                },
                new NameValuePairArgs { Name = "FUNCTIONS_WORKER_RUNTIME", Value = "dotnet-isolated", },
                new NameValuePairArgs { Name = "FUNCTIONS_EXTENSION_VERSION", Value = "~4", },
                new NameValuePairArgs { Name = "SCM_DO_BUILD_DURING_DEPLOYMENT", Value = "0" },
                new NameValuePairArgs
                {
                    Name = "StorageConfig__Endpoint",
                    Value = storageInfra.StorageAccount.Name.Apply(x => $"https://{x}.table.core.windows.net"),
                },
                new NameValuePairArgs
                {
                    Name = "StorageConfig__TableName",
                    Value = storageInfra.FeedbackTableStorage.TableName,
                },
            },
            Cors = new CorsSettingsArgs
            {
                SupportCredentials = true,
                AllowedOrigins = GenerateAllowedOriginsToApi()
            },
        };

        //Create the App Service
        var webApp = new WebApp("feedback-function-app-service", new WebAppArgs
        {
            Kind = "FunctionApp",
            ResourceGroupName = ResourceGroupName,
            ServerFarmId = appServicePlan.Id,
            HttpsOnly = true,
            SiteConfig = functionAppSiteConfig,
            ClientAffinityEnabled = false,
            Identity = new ManagedServiceIdentityArgs
            {
                Type = AzureNative.Web.ManagedServiceIdentityType.SystemAssigned,
            },
        });

        var httpsEndpoint = webApp.HostNames.Apply(x => $"https://{x[0]}");
        var functionsInfra = new FunctionsInfrastructure(
            webApp,
            functionAppSiteConfig,
            appServicePlan,
            httpsEndpoint);

        AssignRbacAccesses(functionsInfra, storageInfra);
        return functionsInfra;
    }

    private ImmutableArray<Output<string>> GenerateAllowedOriginsToApi()
    {
        var builder = new List<Output<string>>();
        builder.Add(Output.Create("https://programmeral.com"));

        return builder.Select(x => x.Apply(y => y.TrimEnd('/').ToLower())).ToImmutableArray();
    }

    private void AssignRbacAccesses(
        FunctionsInfrastructure functionsInfra,
        ServiceStorageInfrastructure storageInfra)
    {
        var functionPrincipalId = functionsInfra.WebApp.Identity.Apply(x => x!.PrincipalId);

        //Allow reading of the Storage Container that stores the Functions Zip Package
        _ = new RoleAssignment("feedback-api-function-blob-storage-contributor-role-assignment",
            new RoleAssignmentArgs
            {
                PrincipalId = functionPrincipalId,
                PrincipalType = PrincipalType.ServicePrincipal,
                RoleDefinitionId =
                    AzureAdRoleIdUtilities.GenerateStorageBlobDataContributorRoleId(ClientConfig.SubscriptionId),
                Scope = storageInfra.StorageAccount.Id
            });

        _ = new RoleAssignment("feedback-api-function-table-storage-contributor-role-assignment",
            new RoleAssignmentArgs
            {
                PrincipalId = functionPrincipalId,
                PrincipalType = PrincipalType.ServicePrincipal,
                RoleDefinitionId =
                    AzureAdRoleIdUtilities.GenerateStorageTableDataContributorRoleId(ClientConfig.SubscriptionId),
                Scope = storageInfra.StorageAccount.Id
            });
    }
}
