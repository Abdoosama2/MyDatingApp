using DatingApp.Application.DTO.Auth;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Application.Services.Services
{
    public class TokenService(IConfiguration config ,UserManager<AppUser> userManager) : ITokenService
    {
        public TokenResult CreateToken(AppUser user, IList<string> roles)
        {
            var tokeKey = config["tokenkey"]?? throw new Exception("can not get the token key") ;

            if (tokeKey.Length < 64)
            {
                throw new Exception("the token key must be greater than 64");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokeKey));
            var claims = new List<Claim>
            {
                new (ClaimTypes.Email,user.Email),
                new (ClaimTypes.NameIdentifier,user.Id)
                    
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResult
            {
                Token = tokenHandler.WriteToken(token),
                Expiration= DateTime.UtcNow.AddMinutes(7),
            };
               
        }

        public async Task<string> GenerateRefreshToken(AppUser user)
        {
            var refreshToken = GenerateSecureString();
            user.RefreshToken= refreshToken;
            user.RefreshTokenExpire = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(user);

            return refreshToken;
        
           
        }

          private static string GenerateSecureString(){
            byte[] bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
