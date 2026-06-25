using DatingApp.Application.DTO;
using DatingApp.Application.DTO.Auth;
using DatingApp.Shared;

namespace DatingApp.Application.Services.Interfaces
{
    public interface IAccountService
    {

        Task<ResultService<AuthResponse>> Register(RegisterDto registerDto);


        Task<ResultService<AuthResponse>> Login(LoginDto loginDto);

        Task<ResultService<AuthResponse>> RefreshTokenAsync(string refreshToken);
        Task Logout(string userId);
    }
}
