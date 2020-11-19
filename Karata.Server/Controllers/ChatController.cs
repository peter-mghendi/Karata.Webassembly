using Karata.Server.Hubs;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Karata.Server.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task SendMessage(ChatMessage message)
        {
            await this._hubContext.Clients.All.SendAsync("MessageReceived", message);
        }
    }
}
