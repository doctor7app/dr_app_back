using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace Common.Services.Interfaces
{
    public interface IServiceGeneric<TEntity, TEntityRead, TEntityCreate,TEntityUpdate, TDbContext>
        where TEntity : class
        where TEntityRead : class
        where TEntityCreate : class
        where TEntityUpdate : class
        where TDbContext : DbContext
    {
        Task<object> Get(Guid id);
        Task<IEnumerable<TEntityRead>> Get();
        Task<object> Create(TEntityCreate entity);
        Task<object> Update(Guid key, Delta<TEntityUpdate> entity);
        Task<object> Delete(Guid id);
    }
}