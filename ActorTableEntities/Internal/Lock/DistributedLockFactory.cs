using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ActorTableEntities.Internal.Lock
{
    internal class DistributedLockFactory
    {

        public static ActorTableEntityOptions Settings { get; private set; }

        public static CloudBlobClient BlobClient { get; private set; }

        public static void Initialise(ActorTableEntityOptions options)
        {
            Settings = options;

            if (Settings == null)
            {
                return;
            }

            if (Settings.StorageConnectionString == null)
            {
                throw new ArgumentNullException(nameof(Settings.StorageConnectionString));
            }

            if (Settings.ContainerName == null)
            {
                throw new ArgumentNullException(nameof(Settings.ContainerName));
            }

            var storageAccount = CloudStorageAccount.Parse(Settings.StorageConnectionString);
            BlobClient = storageAccount.CreateCloudBlobClient();
        }

        public static DistributedLock Get(string key) => new DistributedLock(key);
    }
}
