using Cake.Common;
using Cake.Core;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public record CloudflareWorkersPaths(
    ImmutableArray<CloudflareWorkersPaths.WorkerPaths> Workers,
    string OutputZipFilePath)
{
    public static CloudflareWorkersPaths LoadFromContext(ICakeContext context, string srcDirectory, string buildArtifactsPath)
    {
        var cloudflareWorkersPath = srcDirectory;
        var outputZipFilePath = buildArtifactsPath + "/cloudflare-workers.zip";

        var workers = new List<WorkerPaths>();
        var workerNames = new[] { "public-storage-api", "route-filter-worker" };

        foreach (var workerName in workerNames)
        {
            var projectPath = $"{cloudflareWorkersPath}/{workerName}";
            var workerPath = new WorkerPaths(projectPath, $"{workerName}.js");
            workers.Add(workerPath);
        }

        return new CloudflareWorkersPaths(
            workers.ToImmutableArray(),
            outputZipFilePath);
    }

    public record WorkerPaths(string FolderPath, string TargetJsFileName)
    {
        public string OutputJsFilePath => $"{FolderPath}/dist/worker.js";
    }
}

public record WebsitePaths(
    string ProjectName,
    string PathToSln,
    string ProjectFolder,
    string CsprojFile,
    string OutDir,
    string ZipOutDir,
    string ZipOutFilePath,
    string UnitTestDirectory,
    string UnitTestProj,
    string CoverletOutDir,
    string CustomJsModulesDir)
{
    public static WebsitePaths LoadFromContext(ICakeContext context, string buildConfiguration, string srcDirectory, string buildArtifactsPath)
    {
        var projectName = "ProgrammerAl.Site";
        srcDirectory += $"/{projectName}";
        var pathToSln = srcDirectory + $"/{projectName}.sln";
        var functionProjectDir = srcDirectory + $"/{projectName}";
        var functionCsprojFile = functionProjectDir + $"/{projectName}.csproj";
        var outDir = functionProjectDir + $"/bin/{buildConfiguration}/cake-build-output";
        var zipOutDir = buildArtifactsPath;
        var zipOutFilePath = zipOutDir + $"/programmer-al-site.zip";
        var unitTestDirectory = srcDirectory + $"/UnitTests";
        var unitTestProj = unitTestDirectory + $"/UnitTests.csproj";
        var coverletOutDir = unitTestDirectory + $"/coverlet-coverage-results/";
        var customJsModulesDir = srcDirectory + $"/CustomNpmModules/";

        return new WebsitePaths(
            projectName,
            pathToSln,
            functionProjectDir,
            functionCsprojFile,
            outDir,
            zipOutDir,
            zipOutFilePath,
            unitTestDirectory,
            unitTestProj,
            coverletOutDir,
            customJsModulesDir);
    }
}
