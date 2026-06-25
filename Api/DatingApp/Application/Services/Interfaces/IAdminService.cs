using DatingApp.Application.DTO.Admin;
using DatingApp.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Application.Services.Interfaces
{
    public interface IAdminService
    {

        Task<List<UserWithRoleDto>> GetUsersWithRoles();

        Task<ResultService<List<string>>> EditRole(string userId,string roles);
    }
}
