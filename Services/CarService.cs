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

        public async Task<AllCarsDTO> GetAllCarsAsync(int? skip = null, int? take = null)
        {

            var cars = await _carRepository.GetAllAsync(skip, take);
            var carsCount = await _carRepository.CountAllAsync();

            _logger.LogInformation($"Retrieved {carsCount} cars from the database");

            var allCarsDTO = new AllCarsDTO
            {
                Cars = _mapper.Map<IEnumerable<CarDTO>>(cars),
                TotalCount = carsCount
            };

            return allCarsDTO;
        }

        public async Task<CarDTO?> GetCarByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning($"Car with ID {id} not found");
                return null;
            }
            return _mapper.Map<CarDTO>(car);
        }

        public async Task<CarDTO> CreateCarAsync(CreateUpdateCarDTO CreateUpdateCarDTO)
        {
            var car = _mapper.Map<Car>(CreateUpdateCarDTO);
            var createdCar = await _carRepository.AddAsync(car);
            _logger.LogInformation($"Created new car with ID {createdCar.Id}");
            return _mapper.Map<CarDTO>(createdCar);
        }

        public async Task<CarDTO> UpdateCarAsync(int id, CreateUpdateCarDTO CreateUpdateCarDTO)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning($"Car with ID {id} not found for update");
                throw new KeyNotFoundException($"Car with ID {id} not found");
            }

            _mapper.Map(CreateUpdateCarDTO, car);
            var updatedCar = await _carRepository.UpdateAsync(car);
            _logger.LogInformation($"Updated car with ID {id}");
            return _mapper.Map<CarDTO>(updatedCar);
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning($"Car with ID {id} doesn't exists");
                throw new KeyNotFoundException($"Car with ID {id} not found");
            }

            if (await _carRepository.HasAssignedUsersAsync(id))
            {
                _logger.LogWarning($"Cannot delete car with ID {id} as it has assigned users");
                throw new InvalidOperationException("Cannot delete car that has assigned users");
            }

            await _carRepository.DeleteAsync(car);
            _logger.LogInformation($"Deleted car with ID {id}");
        }

        public async Task<AllCarsDTO> SearchCarsAsync(string? textMatch = null, int? skip = null, int? take = null)
        {
            var cars = new AllCarsDTO();
            cars.Cars = _mapper.Map<IEnumerable<CarDTO>>(await _carRepository.FindCars(textMatch, skip, take));

            cars.TotalCount = await _carRepository.CountFoundCars(textMatch);

            _logger.LogInformation($"Searched cars with text match '{textMatch ?? "null"}' and found {cars.TotalCount}, returning {cars.Cars.Count()}");

            return cars;
        }
    }
}
