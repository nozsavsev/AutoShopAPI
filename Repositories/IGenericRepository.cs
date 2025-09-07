namespace AutoShopAPI.Repositories
{
    public delegate IQueryable<T> QueryCallback<T>(IQueryable<T> query);
    public interface IGenericRepository<T> where T : class
    {

        IQueryable<T> BuildQuery(QueryCallback<T>? queryCallback = null);
        string GetKeyProperty();



        Task<T?> GetByIdAsync(int id, QueryCallback<T>? queryCallback = null);
        Task<List<T>?> GetManyByIdAsync(List<int> ids, QueryCallback<T>? queryCallback = null);


        Task<T> AddAsync(T entity, QueryCallback<T>? queryCallback = null);
        Task<List<T>> AddManyAsync(List<T> entities, QueryCallback<T>? queryCallback = null);

        Task<long> CountAllAsync();

        Task<T> UpdateAsync(T entity, QueryCallback<T>? queryCallback = null);
        Task<List<T>> UpdateManyAsync(List<T> entities, QueryCallback<T>? queryCallback = null);
        
        Task DeleteAsync(T entity);
        Task DeleteManyAsync(List<T> entities);
        Task DeleteManyAsync(List<int> ids);

        Task<bool> ExistsAsync(int id);
    }
}
