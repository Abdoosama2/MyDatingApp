using DatingApp.Application.DTO;
using DatingApp.Application.DTO.MemberDtos;
using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;

namespace DatingApp.Application.Services.Interfaces
{
    public interface IMemberService
    {
        Task<PaginatedResult<MemberDto>> GetMembersAsync(MemberParams memberParams);
        Task<MemberDto?> GetMemberByIdAsync(string id);
        Task<IReadOnlyList<PhotoDto>> GetMemberPhotosAsync(string memberId);
        Task<bool> UpdateMemberAsync(string memberId, UpdateMemberDto updateDto);

        Task<Member> GetMemberForUpdate(string memberId);

        Task<bool> UploadMemberPhotoAsync(string memberId,PhotoDto photo);

        Task <bool> ChangeMemberMainPhoto(string memberId, int photoId);

        Task<bool> DeleteMemberPhoto(string memberId, int photoId);
    }
}
