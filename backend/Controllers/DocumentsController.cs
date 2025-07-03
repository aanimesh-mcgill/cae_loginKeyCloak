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
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;

        public DocumentsController(IDocumentRepository documentRepository, IUserRepository userRepository)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _documentRepository.GetAllAsync();
            var dtos = documents.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound("Document not found");
            return Ok(MapToDto(document));
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Content))
                return BadRequest("Title and content are required");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var document = new Document
            {
                Title = request.Title,
                Content = request.Content,
                CreatedBy = user.AdUsername
            };
            await _documentRepository.CreateAsync(document);
            return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id }, MapToDto(document));
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequest request)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound("Document not found");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            document.Title = request.Title;
            document.Content = request.Content;
            document.UpdatedBy = user.AdUsername;
            document.UpdatedAt = DateTime.UtcNow;
            await _documentRepository.UpdateAsync(document);
            return Ok(MapToDto(document));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound("Document not found");
            await _documentRepository.DeleteAsync(id);
            return Ok(new { message = "Document deleted successfully" });
        }

        private static DocumentDto MapToDto(Document doc)
        {
            return new DocumentDto
            {
                Id = doc.Id,
                Title = doc.Title,
                Content = doc.Content,
                CreatedAt = doc.CreatedAt,
                CreatedBy = doc.CreatedBy,
                UpdatedAt = doc.UpdatedAt,
                UpdatedBy = doc.UpdatedBy
            };
        }
    }
} 