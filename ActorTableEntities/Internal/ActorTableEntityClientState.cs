using System;
using System.Threading.Tasks;
using ActorTableEntities.Internal.Lock;
using ActorTableEntities.Internal.Persistence;
using ActorTableEntities.Internal.Persistence.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities.Internal
{
    internal class ActorTableEntityClientState<T> : IActorTableEntityClientState<T> where T : class, ITableEntity, new()
    {
        private readonly TableEntityProvider tableStorageProvider;

        private readonly DistributedLock mutex;

        public bool IsReleased { get; set; } 

        public bool IsNew { get; set; }

        public T Entity { get; set; }

        public ActorTableEntityClientState(DistributedLock mutex, TableEntityProvider tableStorageProvider)
        {
            this.mutex = mutex;
            this.tableStorageProvider = tableStorageProvider;
        }

        public async Task Hold(string partitionKey, string rowKey)
        {
            await this.mutex.AcquireAsync();

            var entity = await tableStorageProvider.Get<T>(partitionKey, rowKey);

            this.Entity = entity.Result;

            if (this.Entity == null)
            {
                IsNew = true;

                this.Entity = Activator.CreateInstance<T>();
                this.Entity.PartitionKey = tableStorageProvider.ToKey(partitionKey);
                this.Entity.RowKey = tableStorageProvider.ToKey(rowKey);
            }

            this.Entity.ETag = "*"; // ML - Ensure clobbering happens as we have a lock
        }

        public async Task Flush()
        {
            if (IsReleased)
            {
                return;
            }

            try
            {
                if (this.Entity != null)
                {
                    var result = await tableStorageProvider.InsertOrReplace(this.Entity);

                    if (result.StatusCode.IsSuccess())
                    {
                        this.Entity = result.Result;
                    }
                }
            }
            finally
            {
                await this.mutex.ReleaseAsync();
                IsReleased = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Flush();

            this.mutex.Dispose();

            return;
        }
    }
}
