using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LoginSystem.API.DTOs;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;

namespace LoginSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUserRepository _userRepository;

        public ContentController(IContentRepository contentRepository, IUserRepository userRepository)
        {
            _contentRepository = contentRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContent()
        {
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

            var content = new Content
            {
                Title = request.Title,
                Body = request.Body,
                IsPublished = request.IsPublished,
                CreatedBy = user.AdUsername
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

            content.Title = request.Title;
            content.Body = request.Body;
            content.IsPublished = request.IsPublished;
            content.UpdatedBy = user.AdUsername;

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