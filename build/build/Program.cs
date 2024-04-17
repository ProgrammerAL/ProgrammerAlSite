using System;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Common;
using Cake.Common.IO;
using System.IO;
using System.IO.Compression;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Npm;
using Cake.Npm.RunScript;
using System.Threading;
using System.Linq;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.DotNet.Publish;
using NuGet.Packaging;
using System.Collections.Generic;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string Target { get; }
    public string BuildConfiguration { get; }
    public string SrcDirectoryPath { get; }
    public string NugetFilePath { get; }
    public CloudflareWorkersPaths CloudflareWorkersPaths { get; }
    public WebsitePaths WebClientPaths { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Default");
        BuildConfiguration = context.Argument<string>("configuration");
        SrcDirectoryPath = context.Argument("srcDirectoryPath", $"../../src");
        NugetFilePath = context.Argument("nugetConfigFilePath", $"../../src/nuget.config");

        CloudflareWorkersPaths = CloudflareWorkersPaths.LoadFromContext(context, SrcDirectoryPath);
        WebClientPaths = WebsitePaths.LoadFromContext(context, BuildConfiguration, SrcDirectoryPath);
    }
}

[TaskName(nameof(OutputParametersTask))]
public sealed class OutputParametersTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information($"INFO: Current Working Directory: {context.Environment.WorkingDirectory}");

        context.Log.Information($"INFO: {nameof(context.BuildConfiguration)}: {context.BuildConfiguration}");
        context.Log.Information($"INFO: {nameof(context.SrcDirectoryPath)}: {context.SrcDirectoryPath}");
    }
}

[IsDependentOn(typeof(OutputParametersTask))]
[TaskName(nameof(CleanTask))]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.WebClientPaths.OutDir);
    }
}

[IsDependentOn(typeof(CleanTask))]
[TaskName(nameof(BuildTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        //Tasks to build custom NPM packages for public web app
        var buildCustomJsModulesWebAppFuncs = Directory
            .EnumerateDirectories(context.WebClientPaths.CustomJsModulesDir, "*", SearchOption.TopDirectoryOnly)
            .Select(x => new Action(() => PublishCustomJsModule(context, x)));

        //.NET Build tasks
        var buildDotnetAppFuncs = new[]
        {
            () => BuildDotnetApp(context, context.WebClientPaths.PathToSln),
        };

        var buildFuncs = buildDotnetAppFuncs.Concat(buildCustomJsModulesWebAppFuncs).ToArray();

        var runner = Parallel.ForEach(buildFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }

    private void BuildDotnetApp(BuildContext context, string pathToSln)
    {
        context.DotNetRestore(pathToSln, new DotNetRestoreSettings
        {
            ConfigFile = context.NugetFilePath
        });

        context.DotNetBuild(pathToSln, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration
        });
    }

    private void PublishCustomJsModule(BuildContext context, string workingDirectory)
    {
        var settings = new NpmRunScriptSettings
        {
            WorkingDirectory = workingDirectory,
            ScriptName = "publish"
        };

        NpmRunScriptAliases.NpmRunScript(context, settings);
    }
}

[IsDependentOn(typeof(BuildTask))]
[TaskName(nameof(RunUnitTestsTask))]
public sealed class RunUnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var testSettings = new DotNetTestSettings()
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            ArgumentCustomization = (args) => args.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --logger trx")
        };

        var runTestsFuncs = new[]
        {
            () => context.DotNetTest(context.WebClientPaths.UnitTestProj, testSettings),
        };

        var runner = Parallel.ForEach(runTestsFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }
}

[IsDependentOn(typeof(RunUnitTestsTask))]
[TaskName(nameof(PublishApplicationsTask))]
public sealed class PublishApplicationsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildFuncs = new List<Action>
        {
            () => PublishWebClient(context),
        };

        var cloudflareWorkersBuildFuncs = context.CloudflareWorkersPaths.Workers.Select(worker => new Action(() => PublishCloudflareWorker(context, worker))).ToList();
        buildFuncs.AddRange(cloudflareWorkersBuildFuncs);

        var runner = Parallel.ForEach(buildFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }

    private void PublishWebClient(BuildContext context)
    {
        var settings = new DotNetPublishSettings()
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = context.BuildConfiguration,
            OutputDirectory = context.WebClientPaths.OutDir,
        };

        context.DotNetPublish(context.WebClientPaths.CsprojFile, settings);

        //Now that the code is published, create the compressed folder
        if (!Directory.Exists(context.WebClientPaths.ZipOutDir))
        {
            _ = Directory.CreateDirectory(context.WebClientPaths.ZipOutDir);
        }

        if (File.Exists(context.WebClientPaths.ZipOutFilePath))
        {
            File.Delete(context.WebClientPaths.ZipOutFilePath);
        }

        ZipFile.CreateFromDirectory(context.WebClientPaths.OutDir, context.WebClientPaths.ZipOutFilePath);
        context.Log.Information($"Output client web app zip file to: {context.WebClientPaths.ZipOutFilePath}");
    }

    private void PublishCloudflareWorker(BuildContext context, CloudflareWorkersPaths.WorkerPaths workerPaths)
    {
        var settings = new NpmRunScriptSettings
        {
            WorkingDirectory = workerPaths.FolderPath,
            ScriptName = "publish"
        };

        NpmRunScriptAliases.NpmRunScript(context, settings);
    }
}

[IsDependentOn(typeof(PublishApplicationsTask))]
[TaskName(nameof(ZipCloudflareWorkersTask))]
public sealed class ZipCloudflareWorkersTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var directory = new FileInfo(context.CloudflareWorkersPaths.OutputZipFilePath).Directory!;

        if (!directory.Exists)
        {
            directory.Create();
        }

        using (ZipArchive zip = ZipFile.Open(context.CloudflareWorkersPaths.OutputZipFilePath, ZipArchiveMode.Create))
        {
            foreach (var worker in context.CloudflareWorkersPaths.Workers)
            {
                AddZipEntry(zip, worker);
            }
        }
    }

    private void AddZipEntry(ZipArchive zip, CloudflareWorkersPaths.WorkerPaths workerPaths)
    {
        _ = zip.CreateEntryFromFile(workerPaths.OutputJsFilePath, workerPaths.TargetJsFileName);
    }
}

[IsDependentOn(typeof(ZipCloudflareWorkersTask))]
[TaskName("Default")]
public class DefaultTask : FrostingTask
{
}
