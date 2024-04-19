using Pulumi;

using static ProgrammerAl.Site.IaC.StackBuilders.RouteFilterWorker.RouteFilterInfrastructure;

using Cloudflare = Pulumi.Cloudflare;

namespace ProgrammerAl.Site.IaC.StackBuilders.RouteFilterWorker;

public record RouteFilterInfrastructure(
    WorkerInfrastructure ApiInfra)
{

    public record WorkerInfrastructure(
        Cloudflare.WorkerScript Script, Cloudflare.WorkerRoute workerRoute);
}
