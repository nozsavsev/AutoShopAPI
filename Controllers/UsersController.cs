using AutoShopAPI.Models.DTOs;
using AutoShopAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<ActionResult<AllUsersDTO>> SearchUsers([FromQuery] SearchUserFilters filters)
        {
            var result = await _userService.SearchUsersAsync(filters);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 404);
            }
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO createUserDTO)
        {
            var result = await _userService.CreateUserAsync(createUserDTO);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> BulkCreateUsers(IEnumerable<CreateUserDTO> createUserDTOs)
        {
            var result = await _userService.BulkCreateUsersAsync(createUserDTOs);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            var result = await _userService.UpdateUserAsync(id, updateUserDTO);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok();
        }
    }
}
