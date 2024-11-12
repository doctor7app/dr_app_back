using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Services.Interfaces
{
    public interface IRepository<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        #region Async Functions

        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        Task<IEnumerable<TEntity>> GetListAsync();

        Task<IEnumerable<TEntity>> ExecuteStoreQueryAsync(String commandText, params object[] parameters);
        Task<IEnumerable<TEntity>> ExecuteStoreQueryAsync(String commandText, Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includes = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        #endregion


        #region sync Functions

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        TEntity Get(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        IEnumerable<TEntity> ExecuteStoreQuery(String commandText, params object[] parameters);

        IEnumerable<TEntity> ExecuteStoreQuery(String commandText,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entites);
        void Remove(Expression<Func<TEntity, bool>> condition = null);

        void RemoveRange(Expression<Func<TEntity, bool>> condition = null);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entity);

        int Count(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        #endregion

        IQueryable<TEntity> GetListQuery(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        IQueryable<TEntity> GetListAsyncQueryable();

        void ChangeContext(TEntity entity, EntityState state);

        void AttachContext(TEntity entity);

        Task<TEntity> GetAsyncNoTracking(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        Task<TEntity> GetById(Guid id);


        void DetachEntity(TEntity entity, EntityState state);
        Task<int> Complete();
        void Dispose();

    }
}