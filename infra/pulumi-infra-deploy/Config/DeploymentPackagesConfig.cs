﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.IaC.Config;

public record DeploymentPackagesConfig(string UnzippedArtifactsDir)
{
    public string WebsitePath => $"{UnzippedArtifactsDir}/programmer-al-site";
    public string StorageApiWorkerFilePath => $"{UnzippedArtifactsDir}/cloudflare-workers/public-storage-api.js";
    public string RouteFilterWorkerFilePath => $"{UnzippedArtifactsDir}/cloudflare-workers/route-filter-worker.js";
    public string FeedbackApiFunctionsZipFilePath => $"{UnzippedArtifactsDir}/feedback-api.zip";
}

