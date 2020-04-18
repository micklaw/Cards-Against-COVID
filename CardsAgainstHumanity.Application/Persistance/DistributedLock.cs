using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CardsAgainstHumanity.Application.Persistance
{
    public class DistributedLock
    {
        private readonly string key;
        private readonly string storageConnectionString;
        private readonly string storageContainerName;
        private CloudBlobClient blobClient;
        private string leaseId;

        public DistributedLock(string storageConnectionString, string storageContainerName, string key)
        {
            this.key = key;
            this.storageConnectionString = storageConnectionString;
            this.storageContainerName = storageContainerName;
        }

        /// <summary>
        ///     Acquires a lease blob.
        /// </summary>
        public async Task AcquireAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();

            var containerReference = blobClient.GetContainerReference(storageContainerName);
            await containerReference.CreateIfNotExistsAsync();
            var blobReference = containerReference.GetBlockBlobReference(key);

            if (!await blobReference.ExistsAsync()) await blobReference.UploadTextAsync(string.Empty);

            try
            {
                leaseId = await blobReference.AcquireLeaseAsync(TimeSpan.FromSeconds(60));
            }

            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict) throw new InvalidOperationException($"Another job is already running for {key}.");

                throw;
            }
        }


        /// <summary>
        ///     Releases a lease blob.
        /// </summary>  	
        public async Task ReleaseAsync()
        {
            var containerReference = blobClient.GetContainerReference(storageContainerName);
            await containerReference.CreateIfNotExistsAsync();
            var blobReference = containerReference.GetBlockBlobReference(key);

            await blobReference.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
        }

        /// <summary>
        ///     Renews the lease.
        /// </summary>  	
        public async Task RenewAsync()
        {
            var containerClientReference = blobClient.GetContainerReference(storageContainerName);
            await containerClientReference.CreateIfNotExistsAsync();
            var blobReference = containerClientReference.GetBlockBlobReference(key);

            await blobReference.RenewLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
        }
    }
}
