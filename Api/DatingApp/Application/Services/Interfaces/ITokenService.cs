using DatingApp.Application.DTO.Auth;
using DatingApp.Domain.Entites;

namespace DatingApp.Application.Services.Interfaces
{
    public interface ITokenService
    {
        TokenResult CreateToken(AppUser user ,IList<string> roles);

        Task<string> GenerateRefreshToken(AppUser user);
    }
}
