using Karata.Server.Hubs.Clients;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Karata.Server.Hubs
{
    [Authorize]
    public class ChatHub: Hub<IChatClient>
    {
        public async Task Broadcast(ChatMessage message)
        {
            message.Sender = Context.User.Identity.Name;
            await Clients.All.MessageReceived(message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.NewUserConnected($"{Context.User.Identity.Name} has joined the chat.");
        }
    }
}
