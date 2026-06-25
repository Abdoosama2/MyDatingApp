using DatingApp.Application.DTO.Hubs;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Helpers;
using DatingApp.Domain.Entites;
using System.Text.RegularExpressions;
using Group = DatingApp.Application.DTO.Hubs.Group;

namespace DatingApp.Domain.Interfaces
{
    public interface IMessageRepository
    {

        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message?> GetMessageAsync(string messageId);

        public IQueryable<Message> GetMessagesForMember(string memberId,string container);

        Task<IReadOnlyList<Message>> GetMessagesThreadAsync(string currentMemberId,string recipientId);

       

        void AddGroup(Group group);

        Task RemoveConnection(string connectionId);

        Task<Connection?> GetConnection(string connectionId);

        Task<Group?> GetMessageGroup(string group);

        Task<Group?> GetGroupForConnection(string connectionId);





    }
}
