using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CardsAgainstHumanity.Application.Persistance
{
    public class TableStorageProvider
    {
        private readonly CloudTableClient client;

        public TableStorageProvider(string storageConnection)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection.CheckNotNull(nameof(storageConnection)));
            client = storageAccount.CreateCloudTableClient();
        }

        public virtual Task<IList<TableResult>> BulkInsertOrReplace<T>(IList<T> entities, int chunkCount = 100) where T : ITableEntity
        {
            return Chunked(
                entities,
                chunkCount,
                (operation, entity) =>
                {
                    operation.InsertOrReplace(entity);
                });
        }

        public virtual Task<IList<TableResult>> BulkDelete<T>(IList<T> entities, int chunkCount = 100) where T : ITableEntity
        {
            return Chunked(
                entities,
                chunkCount,
                (operation, entity) =>
                {
                    operation.Delete(entity);
                });
        }

        public virtual async Task<TableResult> InsertOrReplace<T>(T entity) where T : ITableEntity
        {
            CloudTable table = CreateIfNotExists<T>();

            TableOperation operation;

            if (entity.ETag == null)
            {
                operation = TableOperation.Insert(entity);
            }
            else
            {
                if (entity.ETag == "*")
                {
                    operation = TableOperation.InsertOrReplace(entity);
                }
                else
                {
                    operation = TableOperation.Replace(entity);
                }
            }

            return await table.ExecuteAsync(operation);
        }

        public virtual Task<TableResult> Delete<T>(T entity) where T : ITableEntity
        {
            CloudTable table = CreateIfNotExists<T>();
            TableOperation deleteOperation = TableOperation.Delete(entity);
            return table.ExecuteAsync(deleteOperation);
        }

        public virtual async Task<TableResult> Get<T>(string partitionKey, string rowKey) where T : ITableEntity
        {
            CloudTable table = CreateIfNotExists<T>();
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            return await table.ExecuteAsync(retrieveOperation);
        }

        /// <summary>
        /// Chunk the request to insert to storage, I would have done this in a Parallel.ForEach, but you are restricted to 100 bulk operation at a time, meh!
        /// </summary>
        private async Task<IList<TableResult>> Chunked<T>(IList<T> entities, int chunkCount, Action<TableBatchOperation, T> preOperationProcessing)
        {
            if (entities == null || !entities.Any())
            {
                return new List<TableResult>();
            }

            CloudTable table = CreateIfNotExists<T>();

            OrderablePartitioner<Tuple<int, int>> chunks = Partitioner.Create(0, entities.Count, chunkCount);

            List<TableResult> results = new List<TableResult>();

            foreach (Tuple<int, int> chunk in chunks.GetDynamicPartitions())
            {
                TableBatchOperation batch = new TableBatchOperation();

                for (int index = chunk.Item1; index < chunk.Item2; index++)
                {
                    preOperationProcessing(batch, entities[index]);
                }

                results.AddRange(await table.ExecuteBatchAsync(batch));
            }

            return results;
        }

        private CloudTable CreateIfNotExists<T>()
        {
            CloudTable table = client.GetTableReference(typeof(T).Name);
            table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
