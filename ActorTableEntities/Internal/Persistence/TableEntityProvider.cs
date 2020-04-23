using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ActorTableEntities.Internal.Persistence.Extensions;
using ActorTableEntities.Internal.Persistence.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities.Internal.Persistence
{
    internal class TableEntityProvider
    {
        private readonly TableStorageProvider storageProvider;

        public TableEntityProvider(TableStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        public Task<PersistResponse<T>> InsertOrReplace<T>(T entities) where T : ITableEntity
        {
            return this.Response<T>(provider => provider.InsertOrReplace(entities));
        }

        public Task<PersistResponse<T>> Get<T>(string partitionKey, string rowKey) where T : ITableEntity
        {
            return this.Response<T>(provider => provider.Get<T>(ToKey(partitionKey), ToKey(rowKey)));
        }

        public string ToKey(string value)
        {
            Regex disallowedTableKeysChars = new Regex(@"[\\\\#%+/?\u0000-\u001F\u007F-\u009F]");

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(nameof(value));
            }

            return disallowedTableKeysChars.Replace(value, string.Empty);
        }

        private async Task<PersistResponse<T>> Response<T>(Func<TableStorageProvider, Task<TableResult>> resultAction) where T : ITableEntity
        {
            try
            {
                var result = await resultAction(this.storageProvider);

                return ToPersistResponseOfType<T>(result);
            }
            catch (StorageException exception)
            {
                return ToPersistResponseOfType<T>(exception);
            }
        }

        private PersistResponse<T> ToPersistResponseOfType<T>(TableResult result) where T : ITableEntity
        {
            result.CheckNotNull(nameof(result));

            var model = default(T);

            if (result.HttpStatusCode.IsSuccess())
            {
                if (result.Result is T entity)
                {
                    model = entity;
                }
            }

            return new PersistResponse<T>()
            {
                Message = result.HttpStatusCode.IsSuccess() ? "OK" : "Failed",
                Result = model,
                StatusCode = result.HttpStatusCode,
                ETag = result.Etag
            };
        }

        private PersistResponse<T> ToPersistResponseOfType<T>(StorageException exception) where T : ITableEntity
        {
            return new PersistResponse<T>()
            {
                Message = $"{exception.RequestInformation.ErrorCode}: {exception.RequestInformation.ExceptionInfo.Message}",
                StatusCode = exception.RequestInformation.HttpStatusCode,
                ETag = exception.RequestInformation.Etag
            };
        }
    }
}
