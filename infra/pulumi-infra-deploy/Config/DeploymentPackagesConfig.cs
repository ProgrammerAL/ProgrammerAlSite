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
    public string PublicWebClientZipPath => $"{RootDir}/public-web-client.zip";

    public string WebsitePath => $"{UnzippedArtifactsDir}/website";
    public string StorageApiWorkerFilePath => $"{UnzippedArtifactsDir}/cloudflare-workers/public-storage-api.js";
}

