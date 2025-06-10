using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulumi.AzureNative.Resources;
using System.Collections.Immutable;
using Pulumi.AzureAD.Outputs;

using AzureAD = Pulumi.AzureAD;
using AzureNative = Pulumi.AzureNative;
using System.IO;

namespace ProgrammerAl.Site.IaC.Utilities;

public static class AzureUtilities
{
    public static Output<string> GetStorageConnectionStringFromStorageAccount(StorageAccount storageAccount)
    {
        var accountName = storageAccount.Name;
        var resourceGroupName = storageAccount.Id.Apply(x => ParseResourceGroupFromResourceId(x));

        return GetStorageConnectionString(resourceGroupName, accountName);
    }

    public static Output<string> GetStorageConnectionString(Input<string> resourceGroupName, Input<string> accountName)
    {
        // Retrieve the primary storage account key.
        var primaryStorageKey = FindPrimaryConnectionStringForStorageAccount(resourceGroupName, accountName);

        // Build the connection string to the storage account.
        return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey};EndpointSuffix=core.windows.net");
    }

    public static Output<string> GetStorageConnectionStringWithoutAccountKey(Input<string> resourceGroupName, Input<string> accountName)
    {
        // Build the connection string to the storage account.
        return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};EndpointSuffix=core.windows.net");
    }

    public static Output<string> BuildStorageQueueUri(StorageAccount parentStorageAccount, Queue queue)
    {
        // Build the uri
        return Output.Format($"https://{parentStorageAccount.Name}.queue.core.windows.net/{queue.Name}");
    }

    public static Output<string> BuildStorageUriToBlobContainer(StorageAccount parentStorageAccount, BlobContainer container)
    {
        // Build the uri
        return Output.Format($"https://{parentStorageAccount.Name}.blob.core.windows.net/{container.Name}");
    }

    public static Output<string> FindPrimaryConnectionStringForStorageAccount(Input<string> resourceGroupName, Input<string> accountName)
    {
        var storageAccountKeys = Output.Tuple(resourceGroupName, accountName).Apply(x =>
        {
            var resourceGroupName = x.Item1;
            var accountName = x.Item2;
            return ListStorageAccountKeys.InvokeAsync(
                new ListStorageAccountKeysArgs
                {
                    ResourceGroupName = resourceGroupName,
                    AccountName = accountName
                });
        });

        return storageAccountKeys.Apply(keys => keys.Keys[0].Value);
    }

    public static Output<string> GetKeyVaultSecretUri(Vault keyVault, Output<string> secretName)
    {
        return keyVault.Name.Apply(name => Output.Format($"@Microsoft.KeyVault(SecretUri=https://{name}.vault.azure.net/secrets/{secretName})"));
    }

    public static string GenerateRoleDefinitionId(string subscriptionId, string roleId)
        => $"/subscriptions/{subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{roleId}";

    public static Output<string> GenerateRoleDefinitionId(string subscriptionId, Output<string> roleId)
        => roleId.Apply(x => $"/subscriptions/{subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{x}");

    public static Output<string> GetBlobContainerUrl(BlobContainer container, StorageAccount account, Output<string> resourceGroupName)
    {
        return Output.Tuple(
            container.Name, account.Name, resourceGroupName).Apply(t =>
        {
            (string containerName, string accountName, string resourceGroupName) = t;
            return Output.Format($"https://{accountName}.blob.core.windows.net/{containerName}");
        });
    }

    public static string GenerateBlobFileResourceName(FileInfo file)
    {
        return file.FullName.Replace("\\", "_").Replace("/", "_").Replace(" ", "_");
    }

    public static string ParseResourceGroupFromResourceId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return string.Empty;
        }

        string resourceGroupHeader = "/resourceGroups/";
        var startIndex = id.IndexOf(resourceGroupHeader, StringComparison.OrdinalIgnoreCase);

        if (startIndex == -1
            && id.StartsWith("resourceGroups/", StringComparison.OrdinalIgnoreCase))
        {
            resourceGroupHeader = "resourceGroups/";
            startIndex = 0;
        }

        if (startIndex >= 0)
        {
            startIndex += resourceGroupHeader.Length;
            var endIndex = id.IndexOf('/', startIndex);
            if (endIndex == -1)
            {
                endIndex = id.Length;
            }
            var length = endIndex - startIndex;

            return id.Substring(startIndex, length);
        }

        return string.Empty;
    }

    public static ImmutableArray<GetUsersUserResult> LoadUsersFromAzureAd(IEnumerable<string> emails)
    {
        var users = AzureAD.GetUsers.InvokeAsync(new AzureAD.GetUsersArgs
        {
            UserPrincipalNames = emails.ToList(),
        }).Result;

        return users.Users;
    }
}
