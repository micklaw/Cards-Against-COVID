using System;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Persistance;
using CardsAgainstHumanity.Application.Persistance.Models.Entities;
using CardsAgainstHumanity.Application.Persistance.Tables;
using CardsAgainstHumanity.Application.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CardsAgainstHumanity.Api.Startup))]
namespace CardsAgainstHumanity.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            builder.Services.AddSingleton(new DistributedLock(connectionString, "cah-mutex", "CardsAgainstHumanity.Api.Startup"));
            builder.Services.AddSingleton(new TableStorageProvider(connectionString));
            builder.Services.AddSingleton<ICardService, CardService>();
            builder.Services.AddSingleton<IPersistanceProvider<Game>, GameTableProvider>();
        }
    }
}
