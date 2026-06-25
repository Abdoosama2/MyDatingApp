using DatingApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DatingApp.SignalR
{

    [Authorize]
    public class PresenceHub (PresenceTracker _tracker):Hub
    {
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.
                SendAsync("UserIsOnline", GetUserId());

            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _tracker.UserDisconnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.SendAsync("userIsOffline",GetUserId());
            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);
            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId()??throw new HubException("cannot get member id");
        }

    }
}
