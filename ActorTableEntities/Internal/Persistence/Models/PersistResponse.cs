using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities.Internal.Persistence.Models
{
    internal class PersistResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public string ETag { get; set; }
    }

    internal class PersistResponse<T> : PersistResponse where T : ITableEntity
    {
        public T Result { get; set; }
    }
}
