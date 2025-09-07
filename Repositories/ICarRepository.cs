using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Repositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<bool> HasAnyUsersAsync(int carId);

        Task<IEnumerable<Car>> FindCarsAsync(SearchCarFilters filters, QueryCallback<Car>? queryCallback = null);

        Task<int> CountFoundCarsAsync(SearchCarFilters filters);
    }
}
