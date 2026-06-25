using AutoMapper;
using DatingApp.Application.DTO;
using DatingApp.Application.DTO.Auth;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Infrastructer.Helpers;
using DatingApp.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Application.Services.Services
{
    public class AccountService(UserManager<AppUser> _userManager,
        ITokenService _tokenService ) : IAccountService
    {

        public async Task<ResultService<AuthResponse>> Register(RegisterDto registerDto)
        {
            var result= new ResultService<AuthResponse>();
            var user = await _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
            {
                return ResultService<AuthResponse>.Failuer("The Email is Already Exist");
            }
            //mapping the register to user
            var newUser = new AppUser
            {

                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Member = new Member
                {
                    DisplayName = registerDto.DisplayName,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    Gender = registerDto.Gender,
                    DateBirth = registerDto.DateOfBirth,

                }
            };

            //creating the user
            var identityResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!identityResult.Succeeded)
            {
                return ResultService<AuthResponse>.Failuer(identityResult.Errors.First().Description);
            }
            //Assigne roles to user

            var roleResult = await _userManager.AddToRoleAsync(newUser, AppRoles.Member);

            if (!roleResult.Succeeded)
            {
                return ResultService<AuthResponse>.Failuer(identityResult.Errors.First().Description);
            }

            var roles = await _userManager.GetRolesAsync(newUser);

            var tokenResult = _tokenService.CreateToken(newUser, roles);
            var refreshToken = await _tokenService.GenerateRefreshToken(newUser);

            result.IsSuccess = true;

            result.Data = new AuthResponse
            {
                Id= user.Id,
                Email= user.Email,
                DisplayName= user.DisplayName,
                Roles=roles.ToList<string>(),
                Token=tokenResult.Token,
                Expiration=tokenResult.Expiration,
                ImageUrl = user.ImageUrl,
                RefreshToken=refreshToken,
              
              
            };

            return result;

        }
        public async Task<ResultService<AuthResponse>> Login(LoginDto loginDto)
        {
            var result = new ResultService<AuthResponse>();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return ResultService<AuthResponse>.Failuer("Incorrect  Email or Password ");
            }

            var identityResult = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!identityResult)
            {
                return ResultService<AuthResponse>.Failuer("Incorrect  Email or Password");
            }
            //getting the user roles 
            var roles= await _userManager.GetRolesAsync(user);
            //creating token for user
            var tokenResult=  _tokenService.CreateToken(user, roles);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            result.IsSuccess = true;
            result.Data = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles.ToList<string>(),
                Token = tokenResult.Token,
                Expiration = tokenResult.Expiration,
                ImageUrl=user.ImageUrl,
                RefreshToken= refreshToken

            };

            return result;
        }

        public async Task Logout(string userId)
        {
            await _userManager.Users.Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.RefreshToken, _ => null)
                .SetProperty(x => x.RefreshTokenExpire, _ => DateTime.MinValue));
            
                
        }

        public async Task<ResultService<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var result = new ResultService<AuthResponse>();
            var user = await _userManager.Users.FirstOrDefaultAsync(x=>x.RefreshToken==refreshToken
            &&x.RefreshTokenExpire>DateTime.UtcNow);

            if (user == null)
            {
                return ResultService<AuthResponse>.Failuer("Cant extract the user from refresh token");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var newRefreshToken= await _tokenService.GenerateRefreshToken(user);
            var accessToken = _tokenService.CreateToken(user, roles);

            result.IsSuccess = true;
            result.Data = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles.ToList<string>(),
                Token = accessToken.Token,
                Expiration = accessToken.Expiration,
                ImageUrl = user.ImageUrl,
                RefreshToken = newRefreshToken

            };

            return result;
        }
    }
}
