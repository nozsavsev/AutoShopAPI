using AutoMapper;
using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;
using AutoShopAPI.Repositories;

namespace AutoShopAPI.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CarService> _logger;

        public CarService(
            ICarRepository carRepository,
            IMapper mapper,
            ILogger<CarService> logger)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<CarDTO>> GetCarByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning("Car with ID {CarId} not found", id);
                return ServiceResult<CarDTO>.NotFound($"Car with ID {id} not found");
            }
            return ServiceResult<CarDTO>.Ok(_mapper.Map<CarDTO>(car));
        }

        public async Task<ServiceResult<CarDTO>> CreateCarAsync(CreateUpdateCarDTO createUpdateCarDTO)
        {
            var car = _mapper.Map<Car>(createUpdateCarDTO);
            var createdCar = await _carRepository.AddAsync(car);
            _logger.LogInformation("Created new car with ID {CarId}", createdCar.Id);
            return ServiceResult<CarDTO>.Ok(_mapper.Map<CarDTO>(createdCar));
        }

        public async Task<ServiceResult<CarDTO>> UpdateCarAsync(int id, CreateUpdateCarDTO createUpdateCarDTO)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning("Car with ID {CarId} not found for update", id);
                return ServiceResult<CarDTO>.NotFound($"Car with ID {id} not found");
            }

            _mapper.Map(createUpdateCarDTO, car);
            var updatedCar = await _carRepository.UpdateAsync(car);
            _logger.LogInformation("Updated car with ID {CarId}", id);
            return ServiceResult<CarDTO>.Ok(_mapper.Map<CarDTO>(updatedCar));
        }

        public async Task<ServiceResult<bool>> DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning("Car with ID {CarId} doesn't exist", id);
                return ServiceResult<bool>.NotFound($"Car with ID {id} not found");
            }

            if (await _carRepository.HasAnyUsersAsync(id))
            {
                _logger.LogWarning("Cannot delete car with ID {CarId} as it has assigned users", id);
                return ServiceResult<bool>.Conflict("Cannot delete car that has assigned users");
            }

            await _carRepository.DeleteAsync(car);
            _logger.LogInformation("Deleted car with ID {CarId}", id);
            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<AllCarsDTO>> SearchCarsAsync(SearchCarFilters filters)
        {
            var cars = new AllCarsDTO();
            
            filters.VerifyAndFix();

            cars.Cars = _mapper.Map<IEnumerable<CarDTO>>(await _carRepository.FindCarsAsync(filters));

            cars.TotalCount = await _carRepository.CountFoundCarsAsync(filters);

            _logger.LogInformation("Searched cars with text match '{TextMatch}' and found {TotalCount}, returning {ReturnedCount}",
                filters.TextMatch ?? "null", cars.TotalCount, cars.Cars.Count());

            return ServiceResult<AllCarsDTO>.Ok(cars);
        }

        public async Task<ServiceResult<IEnumerable<CarDTO>>> BulkCreateCarsAsync(IEnumerable<CreateUpdateCarDTO> createUpdateCarDTOs)
        {
            if (createUpdateCarDTOs == null)
            {
                return ServiceResult<IEnumerable<CarDTO>>.BadRequest("Body is required");
            }

            var dtoList = createUpdateCarDTOs.ToList();
            if (dtoList.Count == 0)
            {
                return ServiceResult<IEnumerable<CarDTO>>.BadRequest("No items to create");
            }

            var carsToCreate = _mapper.Map<List<Car>>(dtoList);
            var created = await _carRepository.AddManyAsync(carsToCreate);
            _logger.LogInformation("Bulk created {Count} cars", created.Count);
            return ServiceResult<IEnumerable<CarDTO>>.Ok(_mapper.Map<IEnumerable<CarDTO>>(created));
        }
    }
}
