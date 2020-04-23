using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace ActorTableEntities
{
    public interface IActorTableEntityClientState<T> : IAsyncDisposable where T : class, ITableEntity, new()
    {
        bool IsReleased { get; set; }

        bool IsNew { get; set; }

        T Entity { get; }

        Task Flush();
    }
}