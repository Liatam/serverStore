

using Microsoft.AspNetCore.SignalR;
using server.Models;

namespace server.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            ConnectedUser.Ids.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUser.Ids.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }


    }
}
