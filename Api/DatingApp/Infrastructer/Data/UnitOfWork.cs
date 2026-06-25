using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructer.Data
{
    public class UnitOfWork(AppDbContext _context) : IUnitOfWork
    {
        private IMemberRepository? _memberRepository;
        private IMessageRepository? _messageRepository;
        private ILikeRepository? _likeRepository;

        public IMemberRepository memberRepository =>
            _memberRepository ??= new MemberRepository(_context);

        public IMessageRepository messageRepository =>
            _messageRepository ??= new MessageRepository(_context);

        public ILikeRepository likeRepository =>
            _likeRepository ??= new LikeRepository(_context);

        public async Task<bool> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync()>0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occured while saving changes");
            }
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
