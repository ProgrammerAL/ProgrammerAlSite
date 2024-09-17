using Pulumi;
using Pulumi.AzureNative.Resources;

using ProgrammerAl.Site.IaC.StackBuilders.AzureResourceGroup;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;

using static ProgrammerAl.Site.IaC.StackBuilders.AzureResourceGroup.AzureResourceGroupInfrasatructure;

public record AzureResourceGroupStackBuilder(
    GlobalConfig GlobalConfig)
{
    public AzureResourceGroupInfrasatructure GenerateResources()
    {
        var resourceGroupInfra = GenerateResourceGroup();

        return new AzureResourceGroupInfrasatructure(resourceGroupInfra);
    }

    private ResourceGroupInfrastructure GenerateResourceGroup()
    {
        var resourceGroup = new ResourceGroup(GlobalConfig.AzureConfig.ResourceGroupName, new ResourceGroupArgs
        {
            Location = GlobalConfig.AzureConfig.Location
        });

        return new ResourceGroupInfrastructure(resourceGroup);
    }
}
