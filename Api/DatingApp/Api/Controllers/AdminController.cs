using DatingApp.Application.DTO;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Application.Services.Services;
using DatingApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
   
    public class AdminController (IAdminService adminService) : BaseApiController
    {
        [Authorize(policy:"RequireAdminRole")]
        [HttpGet("users-with-roles")]

        public async Task<ActionResult<UserDto>> GetUsersWithRoles()
        {
            var users = await adminService.GetUsersWithRoles();

            return Ok(users);
        }

        [Authorize(policy: "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]

        public async Task<ActionResult<ResultService<List<string>>>> EditRoles(string userId, [FromQuery] string roles)
        {
            var result = await adminService.EditRole(userId, roles);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
    }
}
