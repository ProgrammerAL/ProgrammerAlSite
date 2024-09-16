using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Core;
using Azure.Identity;

using Microsoft.Extensions.Options;

namespace ProgrammerAl.Site.FeedbackApi.AzureUtils;

public interface IAzureCredentialsManager
{
    TokenCredential AzureTokenCredential { get; }
}

public class AzureCredentialsManager : IAzureCredentialsManager
{
    public AzureCredentialsManager(IOptions<AzureCredentialsConfig>? azureCredentialsConfig)
    {
        //Only care about these token credentials.
        //  Just set the credential we're using to help with startup performance
        //      so this app doesn't have to spend a lot of time figuring out which credential to use, like what `DefaultAzureCredential()` does
        //  Managed Identity is used when the app is deployed

        var useAzureDeveloperCliCredential = azureCredentialsConfig?.Value?.UseAzureDeveloperCliCredential ?? false;

        if (useAzureDeveloperCliCredential)
        {
            //Use Azure CLI Developer Credential is used when running locally
            //  Set it as the first credential to try to improve performance
            var azureCliCred = new AzureCliCredential();
            AzureTokenCredential = new ChainedTokenCredential(azureCliCred);
        }
        else
        {
            var managedIdCred = new ManagedIdentityCredential();
            AzureTokenCredential = new ChainedTokenCredential(managedIdCred);
        }
    }

    public TokenCredential AzureTokenCredential { get; }
}
