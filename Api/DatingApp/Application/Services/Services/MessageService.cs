using AutoMapper;
using AutoMapper.Execution;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Execptions;
using DatingApp.Application.Helpers;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Application.Services.Services
{
    public class MessageService (IUnitOfWork uwo,IMapper _mapper) : IMessageService
    {
        public async Task<bool> DeleteMessageAsync(string memberId, string messageId)
        {
            var message = await uwo.messageRepository.GetMessageAsync(messageId);

            if (message == null) return false;

            if (message.SenderId != memberId && message.RecipientId != memberId)
            {
                return false;
            }
            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                uwo.messageRepository.DeleteMessage(message);
            }

            return await uwo.Complete();
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParams messageParams)
        {
            var query = uwo.messageRepository.GetMessagesForMember(messageParams.MemberId, messageParams.Container);

            var totalCount = await query.CountAsync();

            var messages = await query
                .Skip((messageParams.PageNumber - 1) * messageParams.PageSize)
                .Take(messageParams.PageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<List<MessageDto>>(messages);

            return new PaginatedResult<MessageDto>
            {
                MetaData= new PaginationMetaData
                {
                    CurrentPage = messageParams.PageNumber,
                    PageSize = messageParams.PageSize,
                    TotalCount = totalCount,
                    TotalPages=(int) Math.Ceiling(totalCount/(double)messageParams.PageSize)
                },
                Items = mappedItems,
               
            };

        }

        public async Task<IReadOnlyList<MessageDto>> GetMessagesThreadAsync(string currentMemberId, string recipientId)
        {
            var messages =await uwo.messageRepository.GetMessagesThreadAsync(currentMemberId, recipientId);

            return _mapper.Map<IReadOnlyList<MessageDto>>(messages);
        }

        public async Task<MessageDto?> SendMessageAsync(string currentMemberId, CreateMessageDto createMessageDto)
        {
            if (currentMemberId == createMessageDto.RecipientId)
            {
                return null;
            }
            var sender = await uwo.memberRepository.GetMemberByIdAsync(currentMemberId);
            var recipient = await uwo.memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
            if (sender == null || recipient == null)
            {
                return null;
            }

            var message = new Message
            {
                SenderId = currentMemberId,
                RecipientId = createMessageDto.RecipientId,
                Content = createMessageDto.Content,
                Sender = sender,
                Recipient = recipient,
                
                
            };

            uwo.messageRepository.AddMessage(message);

            if (await   uwo.Complete())
            {
                return _mapper.Map<MessageDto>(message);
            }

            return null;
        }
    }
}
