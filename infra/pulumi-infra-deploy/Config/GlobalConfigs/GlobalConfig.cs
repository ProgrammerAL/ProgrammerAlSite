using ProgrammerAl.Site.IaC.Utilities;

using Pulumi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Config.GlobalConfigs;

public record ServiceConfig(string Environment, string ServiceVersion);

public record GlobalConfig(
    ServiceConfig ServiceConfig,
    DeploymentPackagesConfig DeploymentPackagesConfig,
    WebClientInfrastructureConfig WebClientInfraConfig,
    CloudflareConfig CloudflareConfig,
    StorageApiConfig StorageApiConfig
)
{
    public static async Task<GlobalConfig> LoadAsync(Pulumi.Config config)
    {
        string secretsKeyVaultUrl = config.Require("secrets-key-vault-url");
        var keyVaultSecrets = await LoadKeyVaultSecretsAsync(secretsKeyVaultUrl);

        string environment = config.Require("environment");
        string serviceVersion = config.Require("service-version");

        var cloudflareConfig = new CloudflareConfigDto
        {
            ApiToken = keyVaultSecrets.CloudflareProviderToken,
            AccountId = config.Require("cloudflare-account-id")
        }
        .GenerateValidConfigObject();

        return new GlobalConfig(
            ServiceConfig: new ServiceConfig(environment, serviceVersion),
            DeploymentPackagesConfig: new DeploymentPackagesConfig(),
            WebClientInfraConfig: config.RequireObject<WebClientInfrastructureConfigDto>("web-client-infra").GenerateValidConfigObject(),
            CloudflareConfig: cloudflareConfig,
            StorageApiConfig: config.RequireObject<StorageApiConfigDto>("storage-api-config").GenerateValidConfigObject()
        );
    }

    private static async Task<KeyVaultSecrets> LoadKeyVaultSecretsAsync(string secretsKeyVaultUrl)
    {
        var keyVaultReader = new KeyVaultReader(secretsKeyVaultUrl);

        var results = await keyVaultReader.ReadSecretsAsync(new[]
        {
            "cloudflare-provider-token"
        });

        return new KeyVaultSecrets(
            CloudflareProviderToken: Output.CreateSecret(results["cloudflare-provider-token"])
            );
    }

    private record KeyVaultSecrets(
        Output<string> CloudflareProviderToken
        );
}
