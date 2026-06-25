using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace DatingApp.Api.Extensions
{
    public static class ClaimPrincipleExtensions
    {
        public static string GetMemberId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)??
                throw new Exception("Cannot get id from Token");
        }
    }
}
