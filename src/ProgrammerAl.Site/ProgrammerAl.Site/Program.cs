using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.DataProviders;
using Microsoft.Extensions.Configuration;
using ProgrammerAl.Site;
using Microsoft.AspNetCore.Components.Web;
using ProgrammerAl.Site.Config;
using ProgrammerAl.Site.HttpClients.FeedbackApi;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiConfig = ConfigureConfig<ApiConfig>(builder);

builder.Services.AddSingleton<FileDownloader>();
builder.Services.AddSingleton<PostDataProvider>();
builder.Services.AddSingleton<PostSummariesProvider>();
builder.Services.AddSingleton<DraftSummariesProvider>();
builder.Services.AddSingleton<TagLinksDataProvider>();

builder.Services.AddSingleton<ISiteLogger, SiteLogger>();

ConfigureHttpClient<IFeedbackApiHttpClient, FeedbackApiHttpClient>(builder, apiConfig.FeedbackApiBaseEndpoint);

await builder.Build().RunAsync();

static T ConfigureConfig<T>(WebAssemblyHostBuilder builder)
    where T : class, new()
{
    var config = new T();
    builder.Configuration.Bind(typeof(T).Name, config);
    _ = builder.Services.AddSingleton(config);
    return config;
}

static void ConfigureHttpClient
<
    TClient,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
    WebAssemblyHostBuilder builder,
    string baseEndpoint)
    where TClient : class
    where TImplementation : class, TClient
{
    var clientBuilder = builder.Services.AddHttpClient<TClient, TImplementation>((serviceProvider, client) =>
    {
        var apiConfig = serviceProvider.GetRequiredService<ApiConfig>();

        if (!baseEndpoint.EndsWith('/'))
        {
            baseEndpoint += "/";
        }

        client.BaseAddress = new Uri(baseEndpoint);
        client.Timeout = apiConfig.HttpTimeout.Value;
    });
}