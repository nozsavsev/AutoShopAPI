using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Services
{
    public interface ICarService
    {
        Task<ServiceResult<AllCarsDTO>> SearchCarsAsync(SearchCarFilters filters);
        Task<ServiceResult<CarDTO>> GetCarByIdAsync(int id);
        Task<ServiceResult<CarDTO>> CreateCarAsync(CreateUpdateCarDTO createUpdateCarDTO);
        Task<ServiceResult<CarDTO>> UpdateCarAsync(int id, CreateUpdateCarDTO createUpdateCarDTO);
        Task<ServiceResult<bool>> DeleteCarAsync(int id);

        Task<ServiceResult<IEnumerable<CarDTO>>> BulkCreateCarsAsync(IEnumerable<CreateUpdateCarDTO> createUpdateCarDTOs);
    }
}
