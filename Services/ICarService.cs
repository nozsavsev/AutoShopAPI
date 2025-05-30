using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Services
{
    public interface ICarService
    {
        Task<AllCarsDTO> GetAllCarsAsync(int? skip = null, int? take = null);
        Task<CarDTO?> GetCarByIdAsync(int id);
        Task<CarDTO> CreateCarAsync(CreateUpdateCarDTO CreateUpdateCarDTO);
        Task<CarDTO> UpdateCarAsync(int id, CreateUpdateCarDTO CreateUpdateCarDTO);
        Task DeleteCarAsync(int id);
    }
}
