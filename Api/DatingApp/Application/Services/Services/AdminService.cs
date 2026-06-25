using DatingApp.Application.DTO.Admin;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Application.Services.Services
{
    public class AdminService(UserManager<AppUser> _userManager) : IAdminService
    {

        public async Task<List<UserWithRoleDto>> GetUsersWithRoles()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserWithRoleDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserWithRoleDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Roles = roles.ToList()
                });
            }

            return result;
        }
        public async Task<ResultService<List<string>>> EditRole(string userId, string roles)
        {
            if (string.IsNullOrEmpty(roles))
            {
                return ResultService<List<string>>.Failuer("you must select at least one role");
            }
            

            var selectedRoles=roles.Split(',');

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return ResultService<List<string>>.Failuer("User don't Exist");
            }
    

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return ResultService<List<string>>.Failuer("failed to add new roles to user");
            }
            

           result =await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return ResultService<List<string>>.Failuer("failed to remove  roles of user");
            }

          var newroles=  await _userManager.GetRolesAsync(user);
            return ResultService<List<string>>.Success(newroles.ToList());

        }

       
    }
}
