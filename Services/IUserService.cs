using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Services
{
    public interface IUserService
    {
        Task<ServiceResult<AllUsersDTO>> SearchUsersAsync(SearchUserFilters filters);
        Task<ServiceResult<UserDTO>> GetUserByIdAsync(int id);
        Task<ServiceResult<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<ServiceResult<UserDTO>> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<ServiceResult<bool>> DeleteUserAsync(int id);

        Task<ServiceResult<IEnumerable<UserDTO>>> BulkCreateUsersAsync(IEnumerable<CreateUserDTO> createUserDTOs);
    }
}
