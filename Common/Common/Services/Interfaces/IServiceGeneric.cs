using Microsoft.AspNetCore.OData.Deltas;

namespace Common.Services.Interfaces
{
    public interface IServiceGeneric<TEntityRead,TEntityCreate,TEntityUpdate>
        where TEntityRead : class
        where TEntityCreate : class
        where TEntityUpdate : class
    {
        Task<object> Get(Guid id);
        Task<IEnumerable<TEntityRead>> Get();
        Task<object> Create(TEntityCreate entity);
        Task<object> Update(Guid key, Delta<TEntityUpdate> entity);
        Task<object> Delete(Guid id);
    }
}