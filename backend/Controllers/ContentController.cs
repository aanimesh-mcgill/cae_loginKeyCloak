using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LoginSystem.API.DTOs;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;
using LoginSystem.API.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LoginSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentRepository contentRepository, IUserRepository userRepository, ILogger<ContentController> logger)
        {
            _contentRepository = contentRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllContent()
        {
            // Log JWT claims for debugging
            JwtClaimsHelper.LogAllClaims(User, _logger, "GetAllContent");
            
            var contents = await _contentRepository.GetAllAsync();
            var contentDtos = contents.Select(MapToDto);
            return Ok(contentDtos);
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedContent()
        {
            var contents = await _contentRepository.GetPublishedAsync();
            var contentDtos = contents.Select(MapToDto);
            return Ok(contentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(Guid id)
        {
            var content = await _contentRepository.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound("Content not found");
            }

            return Ok(MapToDto(content));
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] CreateContentRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Title and body are required");
            }

            // Log JWT claims for debugging
            JwtClaimsHelper.LogAllClaims(User, _logger, "CreateContent");

            // Get username from JWT for audit trail
            var username = JwtClaimsHelper.GetUsername(User);
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Could not extract username from JWT claims");
                return Unauthorized("Invalid user information");
            }

            // Ensure user exists in database (for audit trail purposes)
            var userId = JwtClaimsHelper.GetUserId(User);
            if (userId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    // Check by username as well
                    user = await _userRepository.GetByAdUsernameAsync(username);
                    if (user == null)
                    {
                        // Create user if not exists (for audit trail)
                        var email = JwtClaimsHelper.GetEmail(User);
                        var displayName = JwtClaimsHelper.GetDisplayName(User);
                        var roles = JwtClaimsHelper.GetUserRoles(User);
                        var highestRole = roles.Contains("Admin") ? "Admin" : 
                                         roles.Contains("Editor") ? "Editor" : "Viewer";

                        user = new User
                        {
                            Id = userId.Value,
                            AdUsername = username,
                            Email = email,
                            DisplayName = displayName,
                            Role = highestRole // Store for audit purposes only
                        };
                        await _userRepository.CreateAsync(user);
                        _logger.LogInformation("Created new user in database for audit trail: {Username}", username);
                    }
                    else
                    {
                        _logger.LogInformation("User already exists with username: {Username}", username);
                    }
                }
            }

            var content = new Content
            {
                Title = request.Title,
                Body = request.Body,
                IsPublished = request.IsPublished,
                CreatedBy = username // Use JWT username directly
            };

            await _contentRepository.CreateAsync(content);

            return CreatedAtAction(nameof(GetContentById), new { id = content.Id }, MapToDto(content));
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] UpdateContentRequest request)
        {
            var content = await _contentRepository.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound("Content not found");
            }

            // Log JWT claims for debugging
            JwtClaimsHelper.LogAllClaims(User, _logger, "UpdateContent");

            // Get username from JWT for audit trail
            var username = JwtClaimsHelper.GetUsername(User);
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Could not extract username from JWT claims");
                return Unauthorized("Invalid user information");
            }

            content.Title = request.Title;
            content.Body = request.Body;
            content.IsPublished = request.IsPublished;
            content.UpdatedBy = username; // Use JWT username directly

            await _contentRepository.UpdateAsync(content);

            return Ok(MapToDto(content));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            var content = await _contentRepository.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound("Content not found");
            }

            await _contentRepository.DeleteAsync(id);
            return Ok(new { message = "Content deleted successfully" });
        }

        private static ContentDto MapToDto(Content content)
        {
            return new ContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Body = content.Body,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy,
                IsPublished = content.IsPublished
            };
        }
    }
} 