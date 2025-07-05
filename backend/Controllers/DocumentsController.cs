using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LoginSystem.API.DTOs;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;
using LoginSystem.API.Utils;
using Microsoft.Extensions.Logging;

namespace LoginSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentRepository documentRepository, IUserRepository userRepository, ILogger<DocumentsController> logger)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _logger = logger;
            _logger.LogInformation("[DocumentsController] Constructor called");
            Console.WriteLine("[DocumentsController] Constructor called");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            _logger.LogInformation("[DocumentsController] GetAllDocuments called");
            Console.WriteLine("[DocumentsController] GetAllDocuments called");
            var audClaim = User.FindFirst("aud")?.Value;
            _logger.LogInformation("[DocumentsController] JWT aud claim: {Aud}", audClaim);
            Console.WriteLine($"[DocumentsController] JWT aud claim: {audClaim}");
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("[DocumentsController] JWT claim: {Type} = {Value}", claim.Type, claim.Value);
                Console.WriteLine($"[DocumentsController] JWT claim: {claim.Type} = {claim.Value}");
            }
            _logger.LogInformation("[DocumentsController] User.Identity.IsAuthenticated: {IsAuth}", User.Identity?.IsAuthenticated);
            Console.WriteLine($"[DocumentsController] User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
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

            // Log JWT claims for debugging
            JwtClaimsHelper.LogAllClaims(User, _logger, "CreateDocument");

            // Get user ID using the helper method that tries multiple strategies
            var userId = JwtClaimsHelper.GetUserId(User);
            if (!userId.HasValue)
            {
                _logger.LogWarning("Could not extract valid user ID from JWT claims");
                return Unauthorized("Invalid user identifier");
            }

            // Get username from JWT for audit trail
            var username = JwtClaimsHelper.GetUsername(User);
            _logger.LogInformation("[CreateDocument] Extracted username: {Username}", username);
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("[CreateDocument] Could not extract username from JWT claims");
                return Unauthorized("Invalid user information: username missing");
            }
            if (username.Length > 256)
            {
                _logger.LogWarning("[CreateDocument] Username too long: {Length}", username.Length);
                return BadRequest("Username too long");
            }

            // Ensure user exists in database (for audit trail purposes)
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
                    _logger.LogInformation("[CreateDocument] Creating new user: {@User}", user);
                    await _userRepository.CreateAsync(user);
                    _logger.LogInformation("[CreateDocument] Created new user in database for audit trail: {Username}", username);
                }
                else
                {
                    _logger.LogInformation("[CreateDocument] User already exists with username: {Username}", username);
                }
            }

            _logger.LogInformation("[CreateDocument] Creating document: Title={Title}, ContentLength={ContentLength}, CreatedBy={CreatedBy}", request.Title, request.Content?.Length ?? 0, username);
            var document = new Document
            {
                Title = request.Title,
                Content = request.Content,
                CreatedBy = username // Use JWT username directly
            };
            await _documentRepository.CreateAsync(document);
            _logger.LogInformation("[CreateDocument] Document created successfully: Id={Id}", document.Id);
            return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id }, MapToDto(document));
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequest request)
        {
            _logger.LogInformation("[DocumentsController] UpdateDocument called for Id={Id}, Title={Title}, Content={Content}", id, request.Title, request.Content);
            
            // Log JWT claims for debugging
            JwtClaimsHelper.LogAllClaims(User, _logger, "UpdateDocument");
            
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound("Document not found");

            // Get username from JWT for audit trail
            var username = JwtClaimsHelper.GetUsername(User);
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Could not extract username from JWT claims");
                return Unauthorized("Invalid user information");
            }

            document.Title = request.Title;
            document.Content = request.Content;
            document.UpdatedBy = username; // Use JWT username directly
            document.UpdatedAt = DateTime.UtcNow;
            var updatedDoc = await _documentRepository.UpdateAsync(document);
            _logger.LogInformation("[DocumentsController] After update: Id={Id}, Title={Title}, Content={Content}", updatedDoc.Id, updatedDoc.Title, updatedDoc.Content);
            return Ok(MapToDto(updatedDoc));
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