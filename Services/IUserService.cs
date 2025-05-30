using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Services
{
    public interface IUserService
    {
        Task<AllUsersDTO> GetAllUsersAsync(int? skip = null, int? take = null);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO> CreateUserAsync(CreateUpdateUserDTO createUserDTO);
        Task<UserDTO> UpdateUserAsync(int id, CreateUpdateUserDTO updateUserDTO);
        Task DeleteUserAsync(int id);
    }
}
