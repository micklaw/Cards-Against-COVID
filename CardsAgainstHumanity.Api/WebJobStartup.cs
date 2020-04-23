using System;
using ActorTableEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CardsAgainstHumanity.Api.WebJobStartup))]
namespace CardsAgainstHumanity.Api
{
    public class WebJobStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            builder.AddActorTableEntities(options =>
            {
                options.ContainerName = "cah-mutex";
                options.StorageConnectionString = connectionString;
                options.WithRetry = true;
            });
        }
    }
}
