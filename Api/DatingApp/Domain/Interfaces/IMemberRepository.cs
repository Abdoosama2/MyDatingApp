using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;

namespace DatingApp.Domain.Interfaces
{
    public interface IMemberRepository
    {

        void Update(Member member);

      


        Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);

        Task<Member?> GetMemberByIdAsync(string id);

        Task<IReadOnlyList<Photo>> GetPhotoForMemberAsync(string memberId);

        Task<Member?> GetMemberForUpdate(string id);

        Task<Photo?> GetMemberPhotoById(string memberId,int Photoid);
    }
}
