using System.Collections.Concurrent;
using ActorTableEntities.Internal.Persistence;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace ActorTableEntities.Internal
{
    [Extension("ActorTableEntityBindingExtension")]
    internal class ActorTableEntityBindingExtension : IExtensionConfigProvider
    {
        private readonly ConcurrentDictionary<ActorTableEntityAttribute, IActorTableEntityClient> cachedClients =
            new ConcurrentDictionary<ActorTableEntityAttribute, IActorTableEntityClient>();

        private readonly TableEntityProvider tableEntityProvider;

        public ActorTableEntityBindingExtension(TableEntityProvider tableEntityProvider)
        {
            this.tableEntityProvider = tableEntityProvider;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<ActorTableEntityAttribute>();

            rule.BindToInput(BuildCollector);
        }

        private IActorTableEntityClient BuildCollector(ActorTableEntityAttribute attribute)
        {
            return this.cachedClients.GetOrAdd(attribute, x => new ActorTableEntityClient(tableEntityProvider));
        }
    }
}
