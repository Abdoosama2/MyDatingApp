using DatingApp.Domain.Entites;
using DatingApp.Infrastructer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
   
    public class UserController : BaseApiController
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            this._context= context;
        }

       [Authorize]
        [HttpGet] 
        public async Task<ActionResult<List<AppUser>>> GetAllUser()
        {
            var users= await _context.Users.ToListAsync();

            return Ok(users);
        }

        

    }
}
