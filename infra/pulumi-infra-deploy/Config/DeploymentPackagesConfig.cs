using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Config;

public record DeploymentPackagesConfig()
{
    public string RootDir => "../../release_artifacts";
    public string UnzippedArtifactsDir => "../../unzipped_artifacts";

    public string WebsitePath => $"{UnzippedArtifactsDir}/website";
    public string StorageApiWorkerFilePath => $"{UnzippedArtifactsDir}/cloudflare-workers/public-storage-api.js";
    public string RouteFilterWorkerFilePath => $"{UnzippedArtifactsDir}/cloudflare-workers/route-filter-worker.js";
}

