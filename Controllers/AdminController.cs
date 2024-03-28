using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using server.Hubs;
using server.Models;
using System.Web.Helpers;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _notificationHub;

        public AdminController(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }

        [HttpGet("connectedUsersCount")]
        public IActionResult GetConnectedUsersCount()
        {
            int connectedUsersCount = ConnectedUser.Ids.Count();
            return Ok(new { count = (connectedUsersCount) });
        }
    }
}
