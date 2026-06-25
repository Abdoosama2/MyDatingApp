using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructer.Repositories
{
    public class MemberRepository(AppDbContext context) : IMemberRepository
    {
        public async Task<Member?> GetMemberByIdAsync(string id)
        {
            return  await context.Members.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Member?> GetMemberForUpdate(string id)
        {
           return await context.Members
                .Include(x => x.User)
                .Include(x=>x.Photos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Photo?> GetMemberPhotoById(string memberId, int Photoid)
        {
            return await context.Photos.FirstOrDefaultAsync(x => x.MemberId == memberId&&x.Id==Photoid);
                
        }

        public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
        {
            var query = context.Members.AsQueryable();

            query = query.Where(x => x.Id != memberParams.CurrnetMemberId);

            if(!string.IsNullOrEmpty( memberParams.Gender))
            {
                query = query.Where(x => x.Gender==memberParams.Gender);
            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));
             query=query.Where(x=>x.DateBirth>=minDob&& x.DateBirth<=maxDob);

            query = memberParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };
            return await PaginationHelper.CreateAsync(query, memberParams.PageNumber
                , memberParams.PageSize);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotoForMemberAsync(string memberId)
        {
            return await context.Members.Where(x => x.Id == memberId)
                .SelectMany(x => x.Photos).ToListAsync();
        }

      

        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;  
        }
    }
}
