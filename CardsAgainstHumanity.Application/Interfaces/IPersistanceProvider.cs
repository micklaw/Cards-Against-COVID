using System.Collections.Generic;
using System.Threading.Tasks;
using CardsAgainstHumanity.Application.Persistance.Models;

namespace CardsAgainstHumanity.Application.Interfaces
{
    public interface IPersistanceProvider<TModel> where TModel : class, IVersionable, new()
    {
        Task<IList<PersistResponse>> BulkInsertOrReplace(IList<TModel> models, int chunkCount = 100);

        Task BulkDelete(IList<TModel> models, int chunkCount = 100);

        Task<PersistResponse<TModel>> InsertOrReplace(TModel model);

        Task Delete(TModel model);

        Task<PersistResponse<TModel>> Get(string partitionKey, string rowKey);

        Task<PersistResponse<TModel>> Get(string rowKey);
    }
}
