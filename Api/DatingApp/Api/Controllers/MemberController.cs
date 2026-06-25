using DatingApp.Api.Extensions;
using DatingApp.Application.DTO;
using DatingApp.Application.DTO.MemberDtos;
using DatingApp.Application.Helpers;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class MemberController(IMemberService _memberService,IPhotoService _photoService) : BaseApiController
    {

        [HttpGet]

       
        public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers([FromQuery] MemberParams memberParams)
        {
            memberParams.CurrnetMemberId = User.GetMemberId();
            return  Ok(await _memberService.GetMembersAsync(memberParams));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<MemberDto>> GetMemberById(string id)
        {

            return Ok(await _memberService.GetMemberByIdAsync(id));
        }
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMembers(string id)
        {

            return Ok(await _memberService.GetMemberPhotosAsync(id));
        }


        [HttpPut]

        public async Task<ActionResult> UpdateMember(UpdateMemberDto memberDto)
        {
            var memberId = User.GetMemberId();
            if (memberId==null) return BadRequest("not id found in token");
            var result=await _memberService.UpdateMemberAsync(memberId, memberDto);
            if (!result) return BadRequest();

            return NoContent();
        }

        [HttpPost("add-photo")]

        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm]IFormFile file)
        {
             var memberId = User.GetMemberId();
            if ( String.IsNullOrEmpty(memberId))
            {
                return BadRequest("Can not extract the member id from token");
            }
            var result= await _photoService.UploadPhotoAsync(file);

            if (result.Error!=null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new PhotoDto
            {
                
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = memberId,
            };
            if( await _memberService.UploadMemberPhotoAsync(memberId, photo))
            {
                return Ok(photo);
            }
          
            return BadRequest(photo);
        }

        [HttpPut("set-main-photo/{photoId}")]
 
         public async Task<ActionResult> SetMainPhoto(int photoId)
         {
            var memberId = User.GetMemberId();
            if (String.IsNullOrEmpty(memberId))
            {
                return BadRequest("Can not extract the member id from token");
            }
            var result = await _memberService.ChangeMemberMainPhoto(memberId, photoId);
            if (result)
            {
                return NoContent();
            }
            return BadRequest("An Error Happend while changing the photo");
           
        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeleteMemberPhoto(int photoId)
        {
            var memberId = User.GetMemberId();
            if (String.IsNullOrEmpty(memberId))
            {
                return BadRequest("Can not extract the member id from token");
            }
            if (!await _memberService.DeleteMemberPhoto(memberId, photoId))
            {
                return BadRequest("this photo can Not be deleted");
            }
            return NoContent();
            
        }
    }
}
