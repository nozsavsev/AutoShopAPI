using AutoShopAPI.Models;

namespace AutoShopAPI.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserWithCarAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
    }
}
