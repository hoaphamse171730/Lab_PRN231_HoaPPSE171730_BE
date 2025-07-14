using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Shared
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetAccountId(this ClaimsPrincipal user)
        {
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        public static string? GetEmail(this ClaimsPrincipal user) =>
            user.FindFirstValue(JwtRegisteredClaimNames.Email);

        public static string? GetRole(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.Role);
    }
}
