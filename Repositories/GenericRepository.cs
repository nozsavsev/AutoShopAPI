using AutoShopAPI.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AutoShopAPI.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AutoShopDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(AutoShopDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> BuildQuery(QueryCallback<T>? queryCallback = null)
        {
            var baseQuery = _dbSet
                .AsNoTracking();

            return queryCallback?.Invoke(baseQuery).IgnoreAutoIncludes() ?? baseQuery;
        }

        public virtual string GetKeyProperty()
        {
            var keyName =  _context.Model.FindEntityType(typeof(T))?
                 .FindPrimaryKey()?
                 .Properties.Select(x => x.Name)
                 .FirstOrDefault();
            if (keyName == null)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key defined.");
            }

            return keyName;
        }



        public virtual async Task<T?> GetByIdAsync(int id, QueryCallback<T>? queryCallback = null)
        {
            var query = BuildQuery(queryCallback);

            var keyName = GetKeyProperty();

            return await query.Where(e => EF.Property<int>(e, keyName) == id)
                              .FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>?> GetManyByIdAsync(List<int> ids, QueryCallback<T>? queryCallback = null)
        {
            var query = BuildQuery(queryCallback);

            var keyName = GetKeyProperty();

            return await query.Where(e => ids.Contains(EF.Property<int>(e, keyName))).ToListAsync();
        }


        public virtual async Task<T> AddAsync(T entity, QueryCallback<T>? queryCallback = null)
        {
            var newEntity = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Re-query to load navigation properties according to policy
            var keyName = GetKeyProperty();
            var keyValue = (int)(typeof(T).GetProperty(keyName)!.GetValue(newEntity.Entity)!
                                 ?? throw new InvalidOperationException($"Could not read key value from {typeof(T).Name}"));

            var reloaded = await GetByIdAsync(keyValue, queryCallback);
            if (reloaded == null)
            {
                throw new InvalidOperationException($"Failed to reload newly added {typeof(T).Name} by key {keyValue}.");
            }
            return reloaded;
        }

        public virtual async Task<List<T>> AddManyAsync(List<T> entities, QueryCallback<T>? queryCallback = null)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            var keyName = GetKeyProperty();
            var ids = entities.Select(e => (int)(typeof(T).GetProperty(keyName)!.GetValue(e)!)).ToList();
            var reloaded = await GetManyByIdAsync(ids, queryCallback) ?? new List<T>();
            return reloaded;
        }

        public virtual async Task<long> CountAllAsync()
        {
            return await _dbSet.LongCountAsync();
        }


        public virtual async Task<T> UpdateAsync(T entity, QueryCallback<T>? queryCallback = null)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            var keyName = GetKeyProperty();
            var keyValue = (int)(typeof(T).GetProperty(keyName)!.GetValue(entity)!);
            var reloaded = await GetByIdAsync(keyValue, queryCallback);
            if (reloaded == null)
            {
                throw new InvalidOperationException($"Failed to reload updated {typeof(T).Name} by key {keyValue}.");
            }
            return reloaded;
        }

        public virtual async Task<List<T>> UpdateManyAsync(List<T> entities, QueryCallback<T>? queryCallback = null)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();

            var keyName = GetKeyProperty();
            var ids = entities.Select(e => (int)(typeof(T).GetProperty(keyName)!.GetValue(e)!)).ToList();
            var reloaded = await GetManyByIdAsync(ids, queryCallback) ?? new List<T>();
            return reloaded;
        }


        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteManyAsync(List<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteManyAsync(List<int> ids)
        {
            var keyName = GetKeyProperty();
            var entities = await _dbSet.Where(e => ids.Contains(EF.Property<int>(e, keyName))).ToListAsync();
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            var keyName = GetKeyProperty();
            return await _dbSet.AnyAsync(e => EF.Property<int>(e, keyName) == id);
        }
    }
}