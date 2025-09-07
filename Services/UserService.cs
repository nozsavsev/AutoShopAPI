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

        public async Task<ServiceResult<UserDTO>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return ServiceResult<UserDTO>.NotFound($"User with ID {id} not found");
            }
            return ServiceResult<UserDTO>.Ok(_mapper.Map<UserDTO>(user));
        }

        public async Task<ServiceResult<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            var existingUser = await _userRepository.GetByEmailAsync(createUserDTO.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists", createUserDTO.Email);
                return ServiceResult<UserDTO>.Conflict($"User with email {createUserDTO.Email} already exists");
            }

            if (createUserDTO.CarId.HasValue)
            {
                var carExists = await _carRepository.ExistsAsync(createUserDTO.CarId.Value);
                if (!carExists)
                {
                    _logger.LogWarning("Car with ID {CarId} not found for user creation", createUserDTO.CarId);
                    return ServiceResult<UserDTO>.BadRequest($"Car with ID {createUserDTO.CarId} not found");
                }
            }

            var user = _mapper.Map<User>(createUserDTO);
            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Created new user with ID {UserId}", createdUser.Id);
            return ServiceResult<UserDTO>.Ok(_mapper.Map<UserDTO>(createdUser));
        }

        public async Task<ServiceResult<UserDTO>> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return ServiceResult<UserDTO>.NotFound($"User with ID {id} not found");
            }

            if (user.Email != updateUserDTO.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateUserDTO.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", updateUserDTO.Email);
                    return ServiceResult<UserDTO>.Conflict($"User with email {updateUserDTO.Email} already exists");
                }
            }

            // If password is null, we do not overwrite existing password

            if (updateUserDTO.CarId.HasValue)
            {
                var carExists = await _carRepository.ExistsAsync(updateUserDTO.CarId.Value);
                if (!carExists)
                {
                    _logger.LogWarning("Car with ID {CarId} not found for user update", updateUserDTO.CarId);
                    return ServiceResult<UserDTO>.BadRequest($"Car with ID {updateUserDTO.CarId} not found");
                }
            }

            _mapper.Map(updateUserDTO, user);

            //while password is null, we do not overwrite existing password however if carID is null it means we want to remove the car from user
            user.CarId = updateUserDTO.CarId;

            user.Car = null;//otherwise ef core breaks
            var updatedUser = await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Updated user with ID {UserId}", id);
            return ServiceResult<UserDTO>.Ok(_mapper.Map<UserDTO>(updatedUser));
        }

        public async Task<ServiceResult<bool>> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return ServiceResult<bool>.NotFound($"User with ID {id} not found");
            }
            user.Car = null;
            await _userRepository.DeleteAsync(user);
            _logger.LogInformation("Deleted user with ID {UserId}", id);
            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<AllUsersDTO>> SearchUsersAsync(SearchUserFilters filters)
        {
            filters.VerifyAndFix();

            AllUsersDTO users = new AllUsersDTO();

            users.Users = _mapper.Map<IEnumerable<UserDTO>>(await _userRepository.FindUsersAsync(filters));

            users.TotalCount = await _userRepository.CountFoundUsersAsync(filters);

            _logger.LogInformation("Searched users with text match '{TextMatch}' and found {TotalCount}, returning {ReturnedCount}",
                filters.TextMatch ?? "null", users.TotalCount, users.Users.Count());

            return ServiceResult<AllUsersDTO>.Ok(users);
        }

        public async Task<ServiceResult<IEnumerable<UserDTO>>> BulkCreateUsersAsync(IEnumerable<CreateUserDTO> createUserDTOs)
        {
            if (createUserDTOs == null)
            {
                return ServiceResult<IEnumerable<UserDTO>>.BadRequest("Body is required");
            }

            var dtoList = createUserDTOs.ToList();
            if (dtoList.Count == 0)
            {
                return ServiceResult<IEnumerable<UserDTO>>.BadRequest("No items to create");
            }

            // since request is only for demonstration convenience use only we will automatically correct the emails
            var duplicateEmails = dtoList
                .GroupBy(d => d.Email, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();


            if (duplicateEmails.Count > 0)
            {

                foreach (var email in duplicateEmails)
                {
                    int index = 1;
                    dtoList.Where(d => string.Equals(d.Email, email, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                        .ForEach(d => d.Email = $"{index++}_{d.Email}");
                }


                //usually we return error here...
            }

            duplicateEmails = dtoList
                .GroupBy(d => d.Email, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();


            // Check referenced cars existence for those with CarId
            var carIds = dtoList.Where(d => d.CarId.HasValue).Select(d => d.CarId!.Value).Distinct().ToList();
            foreach (var carId in carIds)
            {
                if (!await _carRepository.ExistsAsync(carId))
                {
                    return ServiceResult<IEnumerable<UserDTO>>.BadRequest($"Car with ID {carId} not found");
                }
            }

            // Check existing users by email
            var existingConflicts = new List<string>();
            foreach (var dto in dtoList)
            {
                var existing = await _userRepository.GetByEmailAsync(dto.Email);
                if (existing != null)
                {
                    existingConflicts.Add(dto.Email);
                }
            }
            if (existingConflicts.Count > 0)
            {
                return ServiceResult<IEnumerable<UserDTO>>.Conflict($"Users with these emails already exist: {string.Join(", ", existingConflicts)}");
            }

            var usersToCreate = _mapper.Map<List<User>>(dtoList);
            var created = await _userRepository.AddManyAsync(usersToCreate);
            _logger.LogInformation("Bulk created {Count} users", created.Count);
            return ServiceResult<IEnumerable<UserDTO>>.Ok(_mapper.Map<IEnumerable<UserDTO>>(created));
        }
    }
}
