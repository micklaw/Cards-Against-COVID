using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Extensions;
using CardsAgainstHumanity.Application.Interfaces;
using CardsAgainstHumanity.Application.Persistance.Models;
using CardsAgainstHumanity.Application.Persistance.Tables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CardsAgainstHumanity.Application.Persistance
{
    public abstract class PersistanceProviderBase<TEntity, TModel> : IPersistanceProvider<TModel> where TEntity : TableEntity where TModel : class, IVersionable, new()
    {
        private readonly TableStorageProvider storageProvider;

        protected PersistanceProviderBase(TableStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        public Task<IList<PersistResponse>> BulkInsertOrReplace(IList<TModel> models, int chunkCount = 100)
        {
            List<TEntity> entities = models?.Select(ToEntity).ToList();
            return this.EmptyResponse(provider => provider.BulkInsertOrReplace(entities, chunkCount));
        }

        public Task BulkDelete(IList<TModel> models, int chunkCount = 100)
        {
            List<TEntity> entities = models?.Select(ToEntity).ToList();
            return this.EmptyResponse(provider => provider.BulkInsertOrReplace(entities, chunkCount));
        }

        public Task<PersistResponse<TModel>> InsertOrReplace(TModel model)
        {
            var entity = ToEntity(model);
            return this.Response(provider => provider.InsertOrReplace(entity));
        }

        public Task Delete(TModel model)
        {
            var entity = ToEntity(model);
            return this.EmptyResponse(provider => provider.Delete(entity));
        }

        public Task<PersistResponse<TModel>> Get(string partitionKey, string rowKey)
        {
            return this.Response(provider => provider.Get<TEntity>(ToKey(partitionKey), ToKey(rowKey)));
        }

        public Task<PersistResponse<TModel>> Get(string rowKey)
        {
            return this.Response(provider => provider.Get<TEntity>(ToKey(GameTableProvider.PartitionKey), ToKey(rowKey)));
        }

        protected abstract TEntity ToEntity(TModel model);

        protected abstract TModel ToModel(TEntity model);

        protected static string ToKey(string value)
        {
            Regex disallowedTableKeysChars = new Regex(@"[\\\\#%+/?\u0000-\u001F\u007F-\u009F]");

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(nameof(value));
            }

            return disallowedTableKeysChars.Replace(value, string.Empty);
        }

        private async Task<PersistResponse<TModel>> Response(Func<TableStorageProvider, Task<TableResult>> resultAction)
        {
            try
            {
                var result = await resultAction(this.storageProvider);

                return ToPersistResponseOfType(result);
            }
            catch (StorageException exception)
            {
                return ToPersistResponseOfType(exception);
            }
        }

        private async Task<PersistResponse> EmptyResponse(Func<TableStorageProvider, Task<TableResult>> resultAction)
        {
            try
            {
                var result = await resultAction(this.storageProvider);

                return ToPersistResponse(result);
            }
            catch (StorageException exception)
            {
                return ToPersistResponse(exception);
            }
        }

        private async Task<IList<PersistResponse<TModel>>> Response(Func<TableStorageProvider, Task<IList<TableResult>>> resultAction)
        {
            var result = await resultAction(this.storageProvider);

            return result?.Select(ToPersistResponseOfType).ToList();
        }

        private async Task<IList<PersistResponse>> EmptyResponse(Func<TableStorageProvider, Task<IList<TableResult>>> resultAction)
        {
            var result = await resultAction(this.storageProvider);

            return result?.Select(ToPersistResponse).ToList();
        }

        private PersistResponse<TModel> ToPersistResponseOfType(TableResult result)
        {
            result.CheckNotNull(nameof(result));

            var model = default(TModel);

            if (result.HttpStatusCode.IsSuccess())
            {
                if (result.Result is TEntity entity)
                {
                    model = ToModel(entity);
                }
            }

            return new PersistResponse<TModel>()
            {
                Message = "OK",
                Result = model,
                StatusCode = result.HttpStatusCode,
                ETag = result.Etag
            };
        }

        private PersistResponse<TModel> ToPersistResponseOfType(StorageException exception)
        {
            return new PersistResponse<TModel>()
            {
                Message = $"{exception.RequestInformation.ErrorCode}: {exception.RequestInformation.ExceptionInfo.Message}",
                StatusCode = exception.RequestInformation.HttpStatusCode,
                ETag = exception.RequestInformation.Etag
            };
        }

        private PersistResponse ToPersistResponse(TableResult result)
        {
            result.CheckNotNull(nameof(result));

            return new PersistResponse()
            {
                Message = "OK",
                StatusCode = result.HttpStatusCode,
                ETag = result.Etag
            };
        }

        private PersistResponse ToPersistResponse(StorageException exception)
        {
            return new PersistResponse()
            {
                Message = $"{exception.RequestInformation.ErrorCode}: {exception.RequestInformation.ExceptionInfo.Message}",
                StatusCode = exception.RequestInformation.HttpStatusCode,
                ETag = exception.RequestInformation.Etag
            };
        }
    }
}
