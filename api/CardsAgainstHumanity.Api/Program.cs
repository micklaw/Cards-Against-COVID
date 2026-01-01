using ActorTableEntities;
using Azure.Data.Tables;
using CardsAgainstHumanity.Api.Services;
using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Configure ActorTableEntities for table storage with blob locking
        services.AddActorTableEntities(options =>
        {
            options.StorageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") 
                                             ?? "UseDevelopmentStorage=true";
            options.ContainerName = "entitylocks";
            options.WithRetry = true;
            options.RetryIntervalMilliseconds = 100;
        });

        // Register application services
        services.AddSingleton<ICardService, CardService>();
        services.AddSingleton<IGameStateService, GameStateService>();
        services.AddSingleton<IPollingService, PollingService>();
    })
    .Build();

host.Run();
