using ActorTableEntities;
using CardsAgainstHumanity.Api.Services;
using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Configure JSON to use camelCase
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configure ActorTableEntities for table storage with blob locking
builder.Services.AddActorTableEntities(Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? "UseDevelopmentStorage=true");

// Register application services
builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<IGameStateService, GameStateService>();
builder.Services.AddSingleton<IPollingService, PollingService>();

builder.Build().Run();
