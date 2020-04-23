using System;
using ActorTableEntities.Internal;
using ActorTableEntities.Internal.Lock;
using ActorTableEntities.Internal.Persistence;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace ActorTableEntities
{
    public static class ActorTableEntityX
    {
        public static IWebJobsBuilder AddActorTableEntities(this IWebJobsBuilder builder, Action<ActorTableEntityOptions> optionsDelegate = null)
        {
            var options = new ActorTableEntityOptions();

            optionsDelegate?.Invoke(options);

            DistributedLockFactory.Initialise(options);

            builder.Services.AddSingleton(new TableStorageProvider(options.StorageConnectionString));
            builder.Services.AddSingleton<TableEntityProvider>();
            builder.AddExtension<ActorTableEntityBindingExtension>();

            return builder;
        }
    }
}
