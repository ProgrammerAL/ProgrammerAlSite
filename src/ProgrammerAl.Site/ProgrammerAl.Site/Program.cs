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

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ConfigureConfig<ApiConfig>(builder);

builder.Services.AddSingleton<FileDownloader>();
builder.Services.AddSingleton<PostDataProvider>();
builder.Services.AddSingleton<PostSummariesProvider>();
builder.Services.AddSingleton<TagLinksDataProvider>();

builder.Services.AddSingleton<ISiteLogger, SiteLogger>();


builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

static T ConfigureConfig<T>(WebAssemblyHostBuilder builder)
    where T : class, new()
{
    var config = new T();
    builder.Configuration.Bind(typeof(T).Name, config);
    _ = builder.Services.AddSingleton(config);
    return config;
}