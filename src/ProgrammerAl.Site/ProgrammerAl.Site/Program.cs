using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.DataProviders;

namespace ProgrammerAl.Site
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            _ = builder.Services.AddSingleton<IConfig>(new HardCodedConfig());
            _ = builder.Services.AddSingleton<FileDownloader>();
            _ = builder.Services.AddSingleton<PostDataProvider>();

            _ = builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
