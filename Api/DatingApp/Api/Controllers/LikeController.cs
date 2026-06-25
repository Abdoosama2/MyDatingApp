using DatingApp.Api.Extensions;
using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    public class LikeController(IUnitOfWork uwo) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]

        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.GetMemberId();
            if (sourceMemberId == null || sourceMemberId == targetMemberId)
            {
                return BadRequest("Invalid Member Id");
            }

            var existingLike = await uwo.likeRepository.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };

                uwo.likeRepository.AddLike(like);
            }
            else
            {
                uwo.likeRepository.DeleteLike(existingLike);
            }
            if(await uwo.Complete())
            {
                return Ok();
            }
            return BadRequest("An error Happend while saving your like");
        }

        [HttpGet("list")]

        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikes()
        {
            var memberId= User.GetMemberId();
            if (string.IsNullOrEmpty(memberId))
            {
                return BadRequest("Invalid member Id");
            }
            var likeIdsList = await uwo.likeRepository.GetCurrentMemberLikeIds(memberId);

            return Ok(likeIdsList);
            
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikesParam likesParam)
        {
            likesParam.MemberId = User.GetMemberId();
            if (string.IsNullOrEmpty(likesParam.MemberId))
            {
                return BadRequest("Invalid member Id");
            }
         

            var members = await uwo.likeRepository.GetMemberLikes(likesParam);

            return Ok(members);

        }


    }
}
