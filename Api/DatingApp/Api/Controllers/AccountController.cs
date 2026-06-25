using DatingApp.Api.Extensions;
using DatingApp.Application.DTO;
using DatingApp.Application.DTO.Auth;
using DatingApp.Application.Extensions;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Infrastructer.Data;
using DatingApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Api.Controllers
{
    public class AccountController(AppDbContext _context ,
        IAccountService accountService) :BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<ResultService<AuthResponse>>> Register(RegisterDto registerDto)
        {
            var result = await accountService.Register(registerDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
           
            SetRefreshToken(result.Data!.RefreshToken);
            return Ok(result);
        }

    
        [HttpPost("login")]
        public async Task<ActionResult<ResultService<AuthResponse>>> Login(LoginDto loginDto)
        {
            var result=  await accountService.Login(loginDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            SetRefreshToken(result.Data!.RefreshToken);
            return Ok(result);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if(refreshToken == null)
            {
                return NoContent();
            }
            var result = await accountService.RefreshTokenAsync(refreshToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            SetRefreshToken(result.Data!.RefreshToken);
            return Ok(result);
        }
    
        private void SetRefreshToken(string  refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly= true,
                Secure= true,
                SameSite=SameSiteMode.Strict,
                Expires=DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("RefreshToken",refreshToken, cookieOptions);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await accountService.Logout(User.GetMemberId());

            Response.Cookies.Delete("RefreshToken");

            return Ok();
        }
    }
}
