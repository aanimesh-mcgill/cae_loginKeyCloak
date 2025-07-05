using System.Security.Claims;
using System.Text.Json;

namespace LoginSystem.API.Utils
{
    public static class JwtClaimsHelper
    {
        /// <summary>
        /// Gets the user's roles from JWT claims (ClaimTypes.Role)
        /// </summary>
        public static List<string> GetUserRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }

        /// <summary>
        /// Checks if the user has any of the specified roles from JWT
        /// </summary>
        public static bool HasRole(ClaimsPrincipal user, params string[] roles)
        {
            var userRoles = GetUserRoles(user);
            return userRoles.Any(role => roles.Contains(role));
        }

        /// <summary>
        /// Gets the user ID from JWT claims, trying multiple strategies
        /// </summary>
        public static Guid? GetUserId(ClaimsPrincipal user)
        {
            // Strategy 1: Try ClaimTypes.NameIdentifier (sub claim)
            var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim != null && Guid.TryParse(nameIdentifierClaim.Value, out var userId1))
            {
                return userId1;
            }

            // Strategy 2: Try "sub" claim directly
            var subClaim = user.FindFirst("sub");
            if (subClaim != null && Guid.TryParse(subClaim.Value, out var userId2))
            {
                return userId2;
            }

            // Strategy 3: Try to extract from "preferred_username" if it's a GUID
            var usernameClaim = user.FindFirst("preferred_username");
            if (usernameClaim != null && Guid.TryParse(usernameClaim.Value, out var userId3))
            {
                return userId3;
            }

            return null;
        }

        /// <summary>
        /// Gets the username from JWT claims
        /// </summary>
        public static string? GetUsername(ClaimsPrincipal user)
        {
            return user.FindFirst("preferred_username")?.Value 
                ?? user.FindFirst(ClaimTypes.Name)?.Value 
                ?? user.Identity?.Name;
        }

        /// <summary>
        /// Gets the email from JWT claims
        /// </summary>
        public static string? GetEmail(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Gets the display name from JWT claims
        /// </summary>
        public static string? GetDisplayName(ClaimsPrincipal user)
        {
            return user.FindFirst("name")?.Value 
                ?? user.FindFirst("preferred_username")?.Value;
        }

        /// <summary>
        /// Logs all JWT claims for debugging purposes
        /// </summary>
        public static void LogAllClaims(ClaimsPrincipal user, ILogger logger, string context = "")
        {
            var prefix = string.IsNullOrEmpty(context) ? "" : $"[{context}] ";
            logger.LogInformation($"{prefix}User Identity: {user.Identity?.Name}, IsAuthenticated: {user.Identity?.IsAuthenticated}");
            
            foreach (var claim in user.Claims)
            {
                logger.LogInformation($"{prefix}Claim: {claim.Type} = {claim.Value}");
            }

            var roles = GetUserRoles(user);
            logger.LogInformation($"{prefix}User Roles: {string.Join(", ", roles)}");
        }
    }
} 