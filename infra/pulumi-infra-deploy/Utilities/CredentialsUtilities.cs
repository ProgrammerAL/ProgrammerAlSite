using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.Storage;

using System;

namespace ProgrammerAl.Site.IaC.Utilities;

public static class CredentialsUtilities
{
    /// <summary>
    /// We use the `DefaultCredential` class for authenticating against Azure
    /// The `EnvironmentCredential` uses the AZURE_* environment variables to know what Service Principal to use
    /// But when using Pulumi in GitHub we have to set the same variables under the `ARM_*` names
    /// So create the AZURE_* variables from the ARM_* variables if needed
    /// </summary>
    public static void SetCredentialEnvironmentVariables()
    {
        var clientId = Environment.GetEnvironmentVariable("ARM_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET");
        var tenantId = Environment.GetEnvironmentVariable("ARM_TENANT_ID");

        if (!string.IsNullOrWhiteSpace(clientId)
            && !string.IsNullOrWhiteSpace(clientSecret)
            && !string.IsNullOrWhiteSpace(tenantId))
        {
            Log.Info($"ARM_* environment variables set. Creating AZURE_* environment variables for use with the `EnvironmentCredential` class");

            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", clientId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", clientSecret);
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", tenantId);
        }
    }
}
