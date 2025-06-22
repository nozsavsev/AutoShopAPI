using AutoShopAPI.Models;

namespace AutoShopAPI.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<IEnumerable<User>> FindUsers(string? textMatch, int? skip = null, int? take = null);

        Task<int> CountFoundUsers(string? textMatch);
    }
}
