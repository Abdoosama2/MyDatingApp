using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;

namespace DatingApp.Domain.Interfaces
{
    public interface ILikeRepository
    {

        Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);

        Task<PaginatedResult<Member>> GetMemberLikes(LikesParam likesParams);

        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);

        void DeleteLike(MemberLike like);

        void AddLike(MemberLike Like);

        


    }
}
