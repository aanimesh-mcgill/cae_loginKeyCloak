using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LoginSystem.API.DTOs;
using LoginSystem.API.Interfaces;

namespace LoginSystem.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.AdUsername,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Role = u.Role,
                LastLogin = u.LastLogin
            });

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.AdUsername,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = user.Role,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.DisplayName))
                user.DisplayName = request.DisplayName;
            
            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;
            
            if (!string.IsNullOrEmpty(request.Role))
                user.Role = request.Role;

            await _userRepository.UpdateAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.AdUsername,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = user.Role,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            await _userRepository.DeleteAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }
    }

    public class UpdateUserRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
} 