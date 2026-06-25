using DatingApp.Application.DTO;
using DatingApp.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DatingApp.Infrastructer.Data
{
    public class SeedData
    {

        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await  userManager.Users.AnyAsync()) return;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructer", "Data", "seedData.json");
            Console.WriteLine($"Reading from: {path}");
            var memberData = await File.ReadAllTextAsync(path);
            Console.WriteLine(memberData);

            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

            if (members == null)
            {
                Console.WriteLine("No members in seed data");
                return;
            }
           
            foreach (var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    DisplayName = member.DisplayName,
                    UserName=member.Email,
                    ImageUrl = member.ImageUrl,
                  
                    Member=new Member
                    {
                        Id=member.Id,
                        DisplayName = member.DisplayName,
                        Descriptions=member.Descriptions,
                        DateBirth=member.DateBirth,
                        ImageUrl=member.ImageUrl,
                        Gender=member.Gender,
                        City=member.City,
                        Country=member.Country,
                        LastActive=member.LastActive,
                        Created=member.Created,
                    }
                };

                user.Member.Photos.Add(new Photo
                {
                    Url=member.ImageUrl,
                    MemberId=member.Id,
                });
             var identityresult=  await userManager.CreateAsync(user, "Pa$$w0rd");

                if (!identityresult.Succeeded)
                {
                    Console.WriteLine(identityresult.Errors.First().Description);
                    continue;
                }

                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin"
            };
            var identityResult = await userManager.CreateAsync(admin, "Pa$$w0rd");
            if (!identityResult.Succeeded)
            {
                Console.WriteLine(identityResult.Errors.First().Description);
            }
            await userManager.AddToRolesAsync(admin, ["Admin","Moderator"]);
        }
    }
}
