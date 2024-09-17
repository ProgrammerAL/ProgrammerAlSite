using Pulumi.AzureAD;
using Pulumi.AzureNative.Resources;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ProgrammerAl.Site.IaC.StackBuilders.AzureResourceGroup.AzureResourceGroupInfrasatructure;

namespace ProgrammerAl.Site.IaC.StackBuilders.AzureResourceGroup;

public record AzureResourceGroupInfrasatructure(
    ResourceGroupInfrastructure ResourceGroupInfra)
{
    public record ResourceGroupInfrastructure(ResourceGroup ResourceGroup);
}
