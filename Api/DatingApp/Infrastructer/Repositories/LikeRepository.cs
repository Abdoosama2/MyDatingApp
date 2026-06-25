using AutoMapper.Execution;
using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Data;
using Microsoft.EntityFrameworkCore;
using Member = DatingApp.Domain.Entites.Member;


namespace DatingApp.Infrastructer.Repositories
{
    public class LikeRepository (AppDbContext _Context) : ILikeRepository
    {
        public void AddLike(MemberLike Like)
        {
          
            _Context.Likes.Add(Like);
           
        }

        public void DeleteLike(MemberLike like)
        {
            _Context.Likes.Remove(like);
        }

        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
           return await _Context.Likes.Where(x => x.SourceMemberId == memberId)
                 .Select(x => x.TargetMemberId).ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await _Context.Likes.FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParam likesParams)
        {
            var query = _Context.Likes.AsQueryable();
            IQueryable<Member> result;

            switch (likesParams.Predicate)
            {
                case "Liked":
                    result = query.Where(x => x.SourceMemberId == likesParams.MemberId)
                        .Select(x => x.TargetMember);
                    break;
                case "LikedBy":
                    result = query.Where(x => x.TargetMemberId == likesParams.MemberId)
                        .Select(x => x.SourceMember);
                    break;
                default:
                    var liksIds = await GetCurrentMemberLikeIds(likesParams.MemberId);
                    result = query.Where(x => x.TargetMemberId == likesParams.MemberId
                    && liksIds.Contains(x.SourceMemberId))
                        .Select(x => x.SourceMember);
                    break;
            }
            return await PaginationHelper.CreateAsync(result, likesParams.PageNumber, likesParams.PageSize);
        }

       

       
    }
}
