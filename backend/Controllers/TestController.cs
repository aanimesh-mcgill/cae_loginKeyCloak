using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LoginSystem.API.Utils;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LoginSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            JwtClaimsHelper.LogAllClaims(User, _logger, "TestClaims");
            
            var result = new
            {
                UserId = JwtClaimsHelper.GetUserId(User),
                Username = JwtClaimsHelper.GetUsername(User),
                Email = JwtClaimsHelper.GetEmail(User),
                DisplayName = JwtClaimsHelper.GetDisplayName(User),
                Roles = JwtClaimsHelper.GetUserRoles(User),
                HasAdminRole = JwtClaimsHelper.HasRole(User, "Admin"),
                HasEditorRole = JwtClaimsHelper.HasRole(User, "Editor"),
                HasViewerRole = JwtClaimsHelper.HasRole(User, "Viewer"),
                AllClaims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList()
            };

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Admin access granted", user = JwtClaimsHelper.GetUsername(User) });
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpGet("editor-or-admin")]
        public IActionResult EditorOrAdmin()
        {
            return Ok(new { message = "Editor or Admin access granted", user = JwtClaimsHelper.GetUsername(User) });
        }

        [Authorize(Roles = "Viewer,Editor,Admin")]
        [HttpGet("any-role")]
        public IActionResult AnyRole()
        {
            return Ok(new { message = "Any role access granted", user = JwtClaimsHelper.GetUsername(User) });
        }
    }
} 