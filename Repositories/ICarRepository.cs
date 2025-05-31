using AutoShopAPI.Models;

namespace AutoShopAPI.Repositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<bool> HasAssignedUsersAsync(int carId);
    }
}
