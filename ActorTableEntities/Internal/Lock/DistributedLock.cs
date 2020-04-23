using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ActorTableEntities.Internal.Lock
{
    internal class DistributedLock : IDisposable
    {
        private readonly CloudBlobContainer containerReference;

        private readonly string key;
        
        private string leaseId;

        private bool disposed = false;

        public DistributedLock(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            this.key = key;
            this.containerReference = DistributedLockFactory.BlobClient.GetContainerReference(DistributedLockFactory.Settings.ContainerName);
        }

        internal async Task AcquireAsync(ActorTableEntityOptions options = null)
        {
            var blobReference = await GetBlobReference();

            if (!await blobReference.ExistsAsync())
            {
                await blobReference.UploadTextAsync(string.Empty);
            }

            try
            {
                if (options?.WithRetry == true)
                {
                    leaseId = await Do(() => blobReference.AcquireLeaseAsync(TimeSpan.FromMilliseconds(options.RetryIntervalMilliseconds)));
                }
                else
                {
                    leaseId = await blobReference.AcquireLeaseAsync(TimeSpan.FromSeconds(60));
                }
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int) HttpStatusCode.Conflict)
            {
                throw new InvalidOperationException($"Another job is already running for {key}.");
            }
        }

        internal async Task ReleaseAsync()
        {
            var blobReference = await GetBlobReference();

            await blobReference.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
        }

        internal async Task RenewAsync()
        {
            var blobReference = await GetBlobReference();

            await blobReference.RenewLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
        }

        private async Task<CloudBlockBlob> GetBlobReference()
        {
            await containerReference.CreateIfNotExistsAsync();
            return containerReference.GetBlockBlobReference(key);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                
            }

            disposed = true;
        }

        private Task<T> Do<T>(
            Func<Task<T>> action,
            int retryInterval = 50,
            int maxAttemptCount = 10)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }

        ~DistributedLock()
        {
            Dispose(false);
        }
    }
}