using AutoShopAPI.Models;

namespace AutoShopAPI.Repositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<bool> HasAssignedUsersAsync(int carId);

        Task<IEnumerable<Car>> FindCars(string? textMatch, int? skip = null, int? take = null);

        Task<int> CountFoundCars(string? textMatch);
    }
}
