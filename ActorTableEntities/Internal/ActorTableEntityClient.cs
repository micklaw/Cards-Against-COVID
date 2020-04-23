using System.Threading.Tasks;
using ActorTableEntities.Internal.Lock;
using ActorTableEntities.Internal.Persistence;
using ActorTableEntities.Internal.Persistence.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities.Internal
{
    internal class ActorTableEntityClient : IActorTableEntityClient
    {
        private readonly TableEntityProvider tableStorageProvider;

        private DistributedLock Mutex { get; set; }

        public ActorTableEntityClient(TableEntityProvider tableStorageProvider)
        {
            this.tableStorageProvider = tableStorageProvider;
        }

        public async Task<T> Get<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            partitionKey.CheckNotNull(nameof(partitionKey));
            rowKey.CheckNotNull(nameof(rowKey));

            /* This will throw a storage exception if it fails to acquire the lock */

            var response = await tableStorageProvider.Get<T>(partitionKey, rowKey);

            return response?.Result;
        }

        public async Task<IActorTableEntityClientState<T>> GetLocked<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            partitionKey.CheckNotNull(nameof(partitionKey));
            rowKey.CheckNotNull(nameof(rowKey));

            /* This will throw a storage exception if it fails to acquire the lock */

            Mutex = DistributedLockFactory.Get(tableStorageProvider.ToKey(partitionKey + rowKey));

            var state = new ActorTableEntityClientState<T>(Mutex, tableStorageProvider);
            await state.Hold(partitionKey, rowKey);
            return state;
        }
    }
}
