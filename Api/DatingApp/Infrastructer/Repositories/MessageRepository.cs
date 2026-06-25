using DatingApp.Application.DTO.Hubs;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructer.Repositories
{
    public class MessageRepository(AppDbContext _context) : IMessageRepository
    {
        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Message.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(x => x.Connections)
                  .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                  .FirstOrDefaultAsync();
        }

        public async Task<Message?> GetMessageAsync(string messageId)
        {
            return await _context.Message.FirstOrDefaultAsync(x=>x.Id== messageId);
        }

        public async Task<Group?> GetMessageGroup(string group)
        {
            return await _context.Groups.Include(x => x.Connections)
                .FirstOrDefaultAsync(g => g.Name == group);
        }

        public IQueryable<Message> GetMessagesForMember(string memberId, string container)
        {
            var query = _context.Message.OrderByDescending(x => x.SentAt)
                .Include(x=>x.Sender).Include(x=>x.Recipient).AsQueryable();

            query = container switch
            {
                "Inbox" => query.Where(x => x.RecipientId == memberId && !x.RecipientDeleted),
                "Outbox"=>query.Where(x=>x.SenderId== memberId && !x.SenderDeleted),
                _=>query.Where(x=>x.RecipientId==memberId&&x.DateRead==null&&!x.RecipientDeleted)
            };
            return query;
        }

        public async Task<IReadOnlyList<Message>> GetMessagesThreadAsync(string currentMemberId, string recipientId)
        {
            await _context.Message.Where(x => x.RecipientId == currentMemberId &&
            x.SenderId == recipientId && x.DateRead == null)
                .ExecuteUpdateAsync(setters=>setters.SetProperty(x=>x.DateRead,DateTime.UtcNow));

            return await _context.Message.Include(x=>x.Sender).Include(x=>x.Recipient).Where(x => (x.SenderId == currentMemberId && x.RecipientId == recipientId &&x.SenderDeleted==false) ||
            (x.SenderId == recipientId && x.RecipientId == currentMemberId &&x.RecipientDeleted==false)).OrderBy(x => x.SentAt)
            .ToListAsync();
        }

        public async Task RemoveConnection(string connectionId)
        {
           await  _context.Connections.Where(x => x.ConnectionId == connectionId)
                 .ExecuteDeleteAsync();
        }

      

      
    }
}
