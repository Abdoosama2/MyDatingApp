using DatingApp.Api.Extensions;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Helpers;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Application.Services.Services;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{

    public class MessageController(IMemberService _memberSerivce, IMessageService _messageService) : BaseApiController
    {

        [HttpPost]

        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
        {
            var senderId = User.GetMemberId();

            if (senderId == messageDto.RecipientId)
            {
                return BadRequest("you can't send message to your Self!");
            }


            var message = await _messageService.SendMessageAsync(senderId, messageDto);

            if (message == null)
            {
                return BadRequest("An Error Occured while sending the message");
            }

            return Ok(message);


        }

        [HttpGet]

        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
        {
            var currentMemberId = User.GetMemberId();
            if (string.IsNullOrEmpty(currentMemberId))
            {
                return BadRequest("Invalid Member Id");
            }
            messageParams.MemberId = currentMemberId;
            var messages = await _messageService.GetMessagesForMemberAsync(messageParams);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]

        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessagesThreads(string recipientId)
        {
            var currentMemberId = User.GetMemberId();
            if (string.IsNullOrEmpty(currentMemberId) || currentMemberId == recipientId)
            {
                return BadRequest("Invalid Member Id");
            }

            var messages = await _messageService.GetMessagesThreadAsync(currentMemberId, recipientId);

            return Ok(messages);
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteMessage(string id)
        {
            var currentMemberId = User.GetMemberId();
            if (string.IsNullOrEmpty(currentMemberId))
            {
                return BadRequest("Invalid Member Id");
            }

            if( await  _messageService.DeleteMessageAsync(currentMemberId, id))
            {
                return NoContent();
            }

            return BadRequest("somtheing Occuerd while deleting");
        }
    }
}
