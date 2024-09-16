using ProgrammerAl.Site.FeedbackFunctionsApp;
using ProgrammerAl.Site.FeedbackFunctionsApp.AzureUtils;
using ProgrammerAl.Site.FeedbackFunctionsApp.Middleware;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        workerApplication.UseMiddleware<ExceptionHandlerMiddleware>();
    })
    .ConfigureAppConfiguration(x =>
    {
#if DEBUG
        x.AddJsonFile("host.json");
        x.AddJsonFile("local.settings.json");
#endif
    })
    .ConfigureServices(serviceCollection =>
    {
        serviceCollection.AddOptionsWithValidateOnStart<StorageConfig, StorageConfigValidateOptions>()
                        .BindConfiguration(nameof(StorageConfig));

        serviceCollection.AddOptionsWithValidateOnStart<AzureCredentialsConfig>()
            .BindConfiguration(nameof(AzureCredentialsConfig));


        serviceCollection.AddSingleton<IAzureCredentialsManager, AzureCredentialsManager>();
    })
    .Build();

host.Run();
