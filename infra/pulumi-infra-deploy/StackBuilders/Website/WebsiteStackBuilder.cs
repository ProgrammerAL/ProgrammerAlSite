﻿using Pulumi;

using System;

using System.IO;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json;

using Cloudflare = Pulumi.Cloudflare;

using static ProgrammerAl.Site.IaC.StackBuilders.Website.WebsiteInfrastructure;
using Pulumi.Command.Local;
using System.Linq;
using ProgrammerAl.Site.IaC.StackBuilders.Website;
using ProgrammerAl.Site.IaC.Utilities;
using ProgrammerAl.Site.IaC.Config.GlobalConfigs;
using ProgrammerAl.Site.IaC.StackBuilders.StorageApi;
using ProgrammerAl.Site.IaC.StackBuilders.FeedbackApi;

public record WebsiteStackBuilder(GlobalConfig GlobalConfig,
    StorageApiInfrastructure StorageApiInfra,
    FeedbackApiInfrastructure FeedbackFunctionsInfra)
{
    public WebsiteInfrastructure GenerateResources()
    {
        var provider = CloudflareUtilities.GenerateCloudflareProvider("cloudflare-provider-website", GlobalConfig.CloudflareConfig.ApiToken);
        var domainEndpoint = GlobalConfig.WebClientInfraConfig.HttpsDomainEndpoint;

        var webClientInfra = UploadWebClientFiles(provider);

        var domainInfra = GenerateDomainEntries(webClientInfra, provider);

        return new WebsiteInfrastructure(webClientInfra, domainInfra, domainEndpoint);
    }

    private CloudflarePagesApp UploadWebClientFiles(
        Cloudflare.Provider provider)
    {
        var projectName = $"programmeral-site-{GlobalConfig.WebClientInfraConfig.Subdomain ?? "prod"}";
        var pagesApp = new Cloudflare.PagesProject(projectName, new Cloudflare.PagesProjectArgs
        {
            Name = projectName,
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            ProductionBranch = "my-prod",
        }, new CustomResourceOptions
        {
            Provider = provider
        });

        //Hack: We do it to guarantee the appsettings files are generated before the command runs to upload all files to Cloudflare
        //      Before this, the files were being generated after the `wrangler pages deploy` command ran
        var webClientDirPathOutput = PrepareAppFilesForUpload();

        _ = webClientDirPathOutput.Apply(webClientDirPath =>
        {
            //Determine checksums of all files
            //  If any are different, the Command.Triggers property will know to run again. Which will upload all files
            var staticFilesChecksums = Directory.GetFiles(webClientDirPath, "*", System.IO.SearchOption.AllDirectories)
            .OrderBy(fileName => fileName)
            .Select(fileName =>
            {
                var fileBytes = System.IO.File.ReadAllBytes(fileName);
                var fileChecksum = System.Security.Cryptography.SHA256.HashData(fileBytes);
                var fileChecksumBase64 = Convert.ToBase64String(fileChecksum);
                return fileChecksumBase64;
            }).ToImmutableArray();

            //Kind of a workaround
            //  Since there's no official way to upload files to Cloudflare Pages, we're using the CLI
            //  The Command.Triggers property will know to run again if any of the files have changed, which will then upload all files
            var createCommand = pagesApp.Name.Apply(projectName =>
            {
                return $"wrangler pages deploy --projectName {projectName} --branch my-prod {webClientDirPath}";
            });
            var environmentVariables = new InputMap<string>
            {
                { "CLOUDFLARE_API_TOKEN", GlobalConfig.CloudflareConfig.ApiToken },
                { "CLOUDFLARE_ACCOUNT_ID", GlobalConfig.CloudflareConfig.AccountId }
            };

            _ = new Command("website-pages-static-files-upload", new CommandArgs
            {
                Create = createCommand,
                Environment = environmentVariables,
                Triggers = staticFilesChecksums
            }, new CustomResourceOptions
            {
                DependsOn = new[] { pagesApp }
            });

            //Have to return something I guess
            return "";
        });

        return new CloudflarePagesApp(pagesApp);
    }

    private Output<string> PrepareAppFilesForUpload()
    {
        return Output.Tuple(
                StorageApiInfra.Domain.HttpsEndpoint,
                FeedbackFunctionsInfra.FeedbackApiFunctionsInfra.HttpsEndpoint
            )
            .Apply(x =>
            {
                var storageApiHttpsEndpoint = x.Item1;
                var feedbackApiHttpsEndpoint = x.Item2;

                JsonNode appSettingsJson = JsonNode.Parse("{}")!;

                AddApiConfigValues(appSettingsJson, storageApiHttpsEndpoint, feedbackApiHttpsEndpoint);

                var contents = StringContentUtilities.GenerateCompressedStringContent(appSettingsJson);

                var webClientDirPath = GlobalConfig.DeploymentPackagesConfig.WebsitePath + "/wwwroot";

                File.WriteAllText($"{webClientDirPath}/appsettings.json", contents.Content);
                File.WriteAllBytes($"{webClientDirPath}/appsettings.json.br", contents.BrotliContent);
                File.WriteAllBytes($"{webClientDirPath}/appsettings.json.gz", contents.GZipContent);

                return webClientDirPath;
            });
    }

    private void AddApiConfigValues(JsonNode appSettingsJson, string httpsStorageApiEndpoint, string feedbackApiHttpsEndpoint)
    {
        var jsonObject = new JsonObject
        {
            ["StorageApiBaseEndpoint"] = $"{httpsStorageApiEndpoint}",
            ["FeedbackApiBaseEndpoint"] = $"{feedbackApiHttpsEndpoint}/api/",
            ["HttpTimeout"] = "00:10:00"
        };

        appSettingsJson["ApiConfig"] = jsonObject;
    }

    private DomainsInfrastructure GenerateDomainEntries(CloudflarePagesApp webClientInfra, Cloudflare.Provider provider)
    {
        string pagesDomainEndpoint = GlobalConfig.WebClientInfraConfig.RootDomain;
        if (!string.IsNullOrWhiteSpace(GlobalConfig.WebClientInfraConfig.Subdomain))
        {
            pagesDomainEndpoint = $"{GlobalConfig.WebClientInfraConfig.Subdomain}.{pagesDomainEndpoint}";
        }

        pagesDomainEndpoint = pagesDomainEndpoint.ToLower();

        var pagesDomain = new Cloudflare.PagesDomain("public-web-app-pages-domain", new()
        {
            AccountId = GlobalConfig.CloudflareConfig.AccountId,
            Name = pagesDomainEndpoint,
            ProjectName = webClientInfra.PagesProject.Name,
        }, new CustomResourceOptions
        {
            Provider = provider
        });

        var record = new Cloudflare.DnsRecord("website-cname", new Cloudflare.DnsRecordArgs
        {
            Name = pagesDomainEndpoint,
            Content = webClientInfra.PagesProject.Subdomain,
            ZoneId = GlobalConfig.WebClientInfraConfig.CloudflareZoneId,
            Proxied = true,
            Type = "CNAME",
            Ttl = 1,//Has to be set to 1 because this is proxied
        }, new CustomResourceOptions
        {
            Provider = provider,
            //Need to register the Pages Domain before making the CNAME record, otherwise it will fail
            DependsOn = new[] { pagesDomain }
        });

        var webClientDomainEndpoint = Pulumi.Output.Create(GlobalConfig.WebClientInfraConfig.HttpsDomainEndpoint);
        return new DomainsInfrastructure(record, pagesDomain, webClientDomainEndpoint);
    }
}
