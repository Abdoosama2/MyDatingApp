using DatingApp.Application.DTO;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;


namespace DatingApp.Application.Extensions
{
    public static class AppUserExtensions
    {
        public static UserDto ToDto(this AppUser user,ITokenService service)
        {
            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Id = user.Id,
                ImageUrl = user.ImageUrl,
               // Token = service.CreateToken(user)
            };
        }
    }
}
