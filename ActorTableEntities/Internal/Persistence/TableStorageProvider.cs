using System.Threading.Tasks;
using ActorTableEntities.Internal.Persistence.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities.Internal.Persistence
{
    internal class TableStorageProvider
    {
        private readonly CloudTableClient client;

        public TableStorageProvider(string storageConnection)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection.CheckNotNull(nameof(storageConnection)));
            client = storageAccount.CreateCloudTableClient();
        }

        public virtual async Task<TableResult> InsertOrReplace<T>(T entity) where T : ITableEntity
        {
            CloudTable table = CreateIfNotExists<T>();
            TableOperation retrieveOperation = TableOperation.InsertOrReplace(entity);

            return await table.ExecuteAsync(retrieveOperation);
        }

        public virtual async Task<TableResult> Get<T>(string partitionKey, string rowKey) where T : ITableEntity
        {
            CloudTable table = CreateIfNotExists<T>();
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            return await table.ExecuteAsync(retrieveOperation);
        }

        private CloudTable CreateIfNotExists<T>()
        {
            CloudTable table = client.GetTableReference(typeof(T).Name);
            table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
