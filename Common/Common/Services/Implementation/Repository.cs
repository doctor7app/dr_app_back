using System.Linq.Expressions;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Services.Implementation
{
    public class Repository<TEntity, TDbContext> : IRepository<TEntity, TDbContext>
        where TEntity : class
        where TDbContext : DbContext
    {
        protected TDbContext Context { get; set; }
        public Repository(TDbContext context)
        {
            Context = context;
        }

        #region Async Functions

        public virtual async Task AddAsync(TEntity entity)
        {
            try
            {
                await Context.Set<TEntity>().AddAsync(entity);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await Context.Set<TEntity>().AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return await query.FirstOrDefaultAsync(condition);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return await query.Where(condition).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> ExecuteStoreQueryAsync(String commandText, params object[] parameters)
        {
            try
            {
                return await Context.Set<TEntity>().FromSqlRaw(commandText, parameters).ToListAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> ExecuteStoreQueryAsync(String commandText,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                return await Context.Set<TEntity>().FromSqlRaw(commandText, includes).ToListAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return await query.Where(condition).CountAsync();
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }


        #endregion

        #region Sync Functions

        public void Add(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                Context.Set<TEntity>().AddRange(entities);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();

                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return query.FirstOrDefault(condition);
                }

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();

                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return query.Where(condition).ToList();
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void Remove(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void RemoveRange(IEnumerable<TEntity> entites)
        {
            try
            {
                Context.Set<TEntity>().RemoveRange(entites);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void Remove(Expression<Func<TEntity, bool>> condition = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (condition != null)
                {
                    Context.Set<TEntity>().Remove(query.FirstOrDefault(condition)!);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void RemoveRange(Expression<Func<TEntity, bool>> condition = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (condition != null)
                {
                    Context.Set<TEntity>().RemoveRange(query.Where(condition));
                }

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Detached;
                Context.Update(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void UpdateRange(IEnumerable<TEntity> entity)
        {
            try
            {
                Context.UpdateRange(entity);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public IQueryable<TEntity> GetListQuery(Expression<Func<TEntity, bool>> condition = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query);
                }
                if (condition != null)
                {
                    return query.Where(condition);
                }
                return query.AsQueryable();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual IEnumerable<TEntity> ExecuteStoreQuery(String commandText, params object[] parameters)
        {
            try
            {
                return Context.Set<TEntity>().FromSqlRaw(commandText, parameters).ToList();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual IEnumerable<TEntity> ExecuteStoreQuery(String commandText,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                return Context.Set<TEntity>().FromSqlRaw(commandText, includes).ToList();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual int Count(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query);
                }

                if (condition != null)
                {
                    return query.Where(condition).Count();
                }

                return query.Count();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        #endregion

        public virtual async Task<IEnumerable<TEntity>> GetListAsync()
        {
            try
            {
                return await Context.Set<TEntity>().AsQueryable().ToListAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public IQueryable<TEntity> GetListAsyncQueryable()
        {
            try
            {
                return Context.Set<TEntity>().AsQueryable();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void ChangeContext(TEntity entity, EntityState state)
        {
            try
            {
                Context.Entry(entity).State = state;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public void AttachContext(TEntity entity)
        {
            try
            {
                Context.Attach(entity);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }

        public virtual async Task<TEntity> GetAsyncNoTracking(Expression<Func<TEntity, bool>> condition = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            try
            {
                IQueryable<TEntity> query = Context.Set<TEntity>();
                if (includes != null)
                {
                    query = includes(query).AsNoTracking();
                }

                if (condition != null)
                {
                    return await query.AsNoTracking().FirstOrDefaultAsync(condition);
                }

                return await query.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
        }


        #region Detach

        public void DetachEntity(TEntity entity, EntityState state)
        {
            try
            {
                Context.Entry(entity).State = state;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region Dispose Save

        public async Task<int> Complete()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

        }
        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}