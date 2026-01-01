using System.Text.Json;
using ActorTableEntities;
using CardsAgainstHumanity.Api.Services;
using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Configure JSON to use camelCase
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        
        // Configure ActorTableEntities for table storage with blob locking
        services.AddActorTableEntities(Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? "UseDevelopmentStorage=true");

        // Register application services
        services.AddSingleton<ICardService, CardService>();
        services.AddSingleton<IGameStateService, GameStateService>();
        services.AddSingleton<IPollingService, PollingService>();
    })
    .Build();

host.Run();
