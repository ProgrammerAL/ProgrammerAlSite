using System.Net.Http;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProgrammerAl.Site.Utilities;

namespace ProgrammerAl.Site
{
#pragma warning disable IDE0058 // Expression value is never used
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<HttpClient>(services => new HttpClient());
            services.AddSingleton<IConfig>(new HardCodedConfig());
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
#pragma warning restore IDE0058 // Expression value is never used
}
