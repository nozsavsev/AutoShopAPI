using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, QueryCallback<User>? queryCallback = null);

        Task<IEnumerable<User>> FindUsersAsync(SearchUserFilters filters, QueryCallback<User>? queryCallback = null);

        Task<int> CountFoundUsersAsync(SearchUserFilters filters);
    }
}
