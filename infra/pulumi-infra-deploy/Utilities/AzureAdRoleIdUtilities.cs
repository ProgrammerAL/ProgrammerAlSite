using Pulumi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Utilities;

/// <summary>
/// Utility methods for generating the Role Definition Id of different Built in Azure Role Ids
/// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
/// </summary>
public static class AzureAdRoleIdUtilities
{
    public const string StorageBlobDataReaderRoleId = "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1";
    public const string StorageBlobDataOwnerRoleId = "b7e6dc6d-f1e8-4753-8033-0f276bb0955b";
    public const string StorageBlobDataContributor = "ba92f5b4-2d11-453d-a403-e96b0029c9fe";
    public const string StorageQueueDataContributor = "974c5e8b-45b9-4653-ba55-5f855dd0fb88";
    public const string StorageTableDataContributor = "0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3";

    public const string AzureBuiltInReaderRoleId = "acdd72a7-3385-48ef-bd42-f606fba81ae7";

    public const string KeyVaultAdminRoleId = "00482a5a-887f-4fb3-b363-3b7fe8e74483";
    public const string KeyVaultSecretsUserRoleId = "4633458b-17de-408a-b874-0445c86b69e6";
    public const string KeyVaultSecretsOfficerRoleId = "b86a8fe4-44ce-4948-aee5-eccb2c155cd7";
    public const string KeyVaultCertificatesOfficerRoleId = "a4417e6f-fecd-4de8-b567-7b0420556985";

    public const string AppConfigurationDataReaderRoleId = "516239f1-63e1-4d78-a4de-a74fb236a071";
    public const string AppConfigurationDataOwnerRoleId = "5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b";

    public static string GenerateAzureReaderRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, AzureBuiltInReaderRoleId);

    public static string GenerateKeyVaultAdminRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, KeyVaultAdminRoleId);

    public static string GenerateKeyVaultSecretsUserRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, KeyVaultSecretsUserRoleId);

    public static string GenerateKeyVaultSecretsOfficerRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, KeyVaultSecretsOfficerRoleId);

    public static string GenerateKeyVaultCertificatesOfficerRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, KeyVaultCertificatesOfficerRoleId);

    public static string GenerateStorageBlobDataOwnerRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, StorageBlobDataOwnerRoleId);

    public static string GenerateStorageBlobDataContributorRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, StorageBlobDataContributor);

    public static string GenerateStorageBlobDataReaderRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, StorageBlobDataReaderRoleId);

    public static string GenerateStorageQueueDataContributorRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, StorageQueueDataContributor);

    public static string GenerateStorageTableDataContributorRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, StorageTableDataContributor);

    public static string GenerateAppConfigurationReaderRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, AppConfigurationDataReaderRoleId);

    public static string GenerateAppConfigurationOwnerRoleId(string subscriptionId)
        => AzureUtilities.GenerateRoleDefinitionId(subscriptionId, AppConfigurationDataOwnerRoleId);
}
