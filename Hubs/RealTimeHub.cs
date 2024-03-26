namespace server.Hubs
{


    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class RealTimeHub : Hub
    {
        public static int ConnectedUsers = 0;

        public override Task OnConnectedAsync()
        {
            ConnectedUsers++;
            Clients.All.SendAsync("UserCountUpdated", ConnectedUsers);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers--;
            Clients.All.SendAsync("UserCountUpdated", ConnectedUsers);
            return base.OnDisconnectedAsync(exception);
        }
    }
}