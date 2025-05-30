using AutoMapper;
using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;
using AutoShopAPI.Repositories;

namespace AutoShopAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ICarRepository carRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _carRepository = carRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AllUsersDTO> GetAllUsersAsync(int? skip = null, int? take = null)
        {
            var users = await _userRepository.GetAllAsync(skip, take);
            var usersCount = await _userRepository.CountAllAsync();

            var allUsersDTO = new AllUsersDTO();

            allUsersDTO.Users = _mapper.Map<IEnumerable<UserDTO>>(users);
            allUsersDTO.TotalCount = usersCount;

            return allUsersDTO;
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithCarAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found");
                return null;
            }
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUpdateUserDTO createUserDTO)
        {
            var existingUser = await _userRepository.GetByEmailAsync(createUserDTO.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"User with email {createUserDTO.Email} already exists");
                throw new InvalidOperationException($"User with email {createUserDTO.Email} already exists");
            }

            if (createUserDTO.CarId.HasValue)
            {
                var carExists = await _carRepository.ExistsAsync(createUserDTO.CarId.Value);
                if (!carExists)
                {
                    _logger.LogWarning("Car with ID {CarId} not found for user creation", createUserDTO.CarId);
                    throw new KeyNotFoundException($"Car with ID {createUserDTO.CarId} not found");
                }
            }

            var user = _mapper.Map<User>(createUserDTO);
            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Created new user with ID {UserId}", createdUser.Id);
            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, CreateUpdateUserDTO updateUserDTO)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            if (user.Email != updateUserDTO.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateUserDTO.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", updateUserDTO.Email);
                    throw new InvalidOperationException($"User with email {updateUserDTO.Email} already exists");
                }
            }

            if (updateUserDTO.Password == null)
            {
                updateUserDTO.Password = user.Password;
            }

            if (updateUserDTO.CarId.HasValue)
            {
                var carExists = await _carRepository.ExistsAsync(updateUserDTO.CarId.Value);
                if (!carExists)
                {
                    _logger.LogWarning("Car with ID {CarId} not found for user update", updateUserDTO.CarId);
                    throw new KeyNotFoundException($"Car with ID {updateUserDTO.CarId} not found");
                }
            }

            _mapper.Map(updateUserDTO, user);
            var updatedUser = await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Updated user with ID {UserId}", id);
            return _mapper.Map<UserDTO>(updatedUser);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            await _userRepository.DeleteAsync(user);
            _logger.LogInformation("Deleted user with ID {UserId}", id);
        }
    }
}
