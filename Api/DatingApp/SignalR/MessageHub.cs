using AutoMapper;
using DatingApp.Api.Extensions;
using DatingApp.Application.DTO.Hubs;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DatingApp.SignalR
{
    public class MessageHub(IUnitOfWork uwo
        ,IHubContext<PresenceHub> presenceHub,IMapper _mapper) :Hub
    {
        public override async Task OnConnectedAsync()   
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext?.Request?.Query["userId"].ToString()??throw new HubException("the other user not found");


            var groupName = GetGroupName(GetUserId(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await AddToGroup(groupName);

            var messages = await uwo.messageRepository.GetMessagesThreadAsync(GetUserId(),otherUser);
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);


            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messageDtos);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            if (GetUserId() == createMessageDto.RecipientId)
            {
                throw new HubException("cannot send the message");
            }
            var sender = await uwo.memberRepository.GetMemberByIdAsync(GetUserId());
            var recipient = await uwo.memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
            if (sender == null || recipient == null)
            {
                throw new HubException("cannot send the message");
            }

            var message = new Message
            {
                SenderId = GetUserId(),
                RecipientId = createMessageDto.RecipientId,
                Content = createMessageDto.Content,
                Sender = sender,
                Recipient = recipient
                
            };

            var groupName = GetGroupName(sender.Id, recipient.Id);

            var group = await uwo.messageRepository.GetMessageGroup(groupName);
            var userIngroup = group != null && group.Connections.Any(x => x.UserId == message.RecipientId);
           if (userIngroup)
            {
                message.DateRead = DateTime.UtcNow;
            }


            uwo.messageRepository.AddMessage(message);

            if (await uwo.Complete())
            {
               
                var messageDto= _mapper.Map<MessageDto>(message);
                await Clients.Group(groupName).SendAsync("NewMessage", messageDto);

                var connections = await PresenceTracker.GetConnectionsForUsers(recipient.Id);
                if(connections!=null && connections.Count>0 &&!userIngroup)
                {
                    await presenceHub.Clients.Clients(connections)
                        .SendAsync("NewMessageReceived", messageDto);
                }
            }

            return;

        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await uwo.messageRepository.GetMessageGroup(groupName);
            var connection =  new Connection(Context.ConnectionId, GetUserId());

            if (group == null)
            {
                group = new Group(groupName);
                uwo.messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await uwo.Complete();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await uwo.messageRepository.RemoveConnection(Context.ConnectionId);
          await base.OnDisconnectedAsync(exception);
        }
        private static string GetGroupName(string? caller, string? otherUser)
        {
            var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
            return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("cannot get member id");
        }
    }
}
