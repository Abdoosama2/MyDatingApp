using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Helpers;


namespace DatingApp.Application.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto?> SendMessageAsync(string currentMemberId, CreateMessageDto createMessageDto);
        Task<bool> DeleteMessageAsync(string memberId, string messageId);
        Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParams messageParams);
        Task<IReadOnlyList<MessageDto>> GetMessagesThreadAsync(string currentMemberId, string recipientId);
    }
}
