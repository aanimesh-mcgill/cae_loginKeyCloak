using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LoginSystem.API.DTOs;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;
using LoginSystem.API.Services;

namespace LoginSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IActiveDirectoryService _adService;
        private readonly IUserRepository _userRepository;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly JwtService _jwtService;

        public AuthController(
            IActiveDirectoryService adService,
            IUserRepository userRepository,
            ILoginHistoryRepository loginHistoryRepository,
            JwtService jwtService)
        {
            _adService = adService;
            _userRepository = userRepository;
            _loginHistoryRepository = loginHistoryRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required");
            }

            // Validate credentials against Active Directory
            var isValid = await _adService.ValidateCredentialsAsync(request.Username, request.Password);
            
            if (!isValid)
            {
                // Log failed login attempt
                await LogLoginAttempt(request.Username, false, "Invalid credentials");
                return Unauthorized("Invalid credentials");
            }

            // Get user info from AD
            var adUserInfo = await _adService.GetUserInfoAsync(request.Username);
            if (adUserInfo == null)
            {
                await LogLoginAttempt(request.Username, false, "User not found in AD");
                return Unauthorized("User not found in Active Directory");
            }

            // Get or create user in database
            var user = await _userRepository.GetByAdUsernameAsync(adUserInfo.Username);
            if (user == null)
            {
                // Create new user
                user = new User
                {
                    AdUsername = adUserInfo.Username,
                    DisplayName = adUserInfo.DisplayName,
                    Email = adUserInfo.Email,
                    Role = DetermineUserRole(adUserInfo.Groups),
                    LastLogin = DateTime.UtcNow
                };
                await _userRepository.CreateAsync(user);
            }
            else
            {
                // Update existing user
                user.DisplayName = adUserInfo.DisplayName;
                user.Email = adUserInfo.Email;
                user.LastLogin = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }

            // Log successful login
            await LogLoginAttempt(user.AdUsername, true, null);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            var response = new LoginResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.AdUsername,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Role = user.Role,
                    LastLogin = user.LastLogin
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var permissions = GetPermissionsForRole(user.Role);

            var response = new CurrentUserResponse
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.AdUsername,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Role = user.Role,
                    LastLogin = user.LastLogin
                },
                Permissions = permissions
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In a real application, you might want to blacklist the token
            // For now, we'll just return success - the client should remove the token
            return Ok(new { message = "Logged out successfully" });
        }

        private async Task LogLoginAttempt(string username, bool success, string? failureReason)
        {
            var loginHistory = new LoginHistory
            {
                Username = username,
                Success = success,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                FailureReason = failureReason
            };

            await _loginHistoryRepository.CreateAsync(loginHistory);
        }

        private string DetermineUserRole(List<string> adGroups)
        {
            // Map AD groups to application roles
            if (adGroups.Any(g => g.Contains("Admin", StringComparison.OrdinalIgnoreCase) || 
                                  g.Contains("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                return "Admin";
            }
            else if (adGroups.Any(g => g.Contains("Editor", StringComparison.OrdinalIgnoreCase)))
            {
                return "Editor";
            }
            else
            {
                return "Viewer";
            }
        }

        private List<string> GetPermissionsForRole(string role)
        {
            return role.ToLower() switch
            {
                "admin" => new List<string> 
                { 
                    "content:read", "content:create", "content:update", "content:delete",
                    "users:read", "users:update", "users:delete",
                    "system:admin"
                },
                "editor" => new List<string> 
                { 
                    "content:read", "content:create", "content:update" 
                },
                "viewer" => new List<string> 
                { 
                    "content:read" 
                },
                _ => new List<string> { "content:read" }
            };
        }
    }
} 