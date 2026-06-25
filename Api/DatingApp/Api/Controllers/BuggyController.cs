using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {

        [HttpGet("auth")]

        public ActionResult GetAuth()
        {
            return Unauthorized();
        }
        [HttpGet("not-found")]

        public ActionResult GetNotFound()
        {   
            return NotFound();
        }
        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {
            throw new Exception("this is a server error");

        }

        [HttpGet("bad-request")]

        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("admin")]

        public ActionResult GetAdmin()
        {
            return Ok("i hop u admin ");
        }
    }
}
