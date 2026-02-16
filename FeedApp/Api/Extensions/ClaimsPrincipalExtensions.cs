using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FeedApp.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID claim not found in token.");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}
