using Karata.Server.Hubs.Clients;
using Karata.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Karata.Server.Hubs
{
    public class ChatHub: Hub<IChatClient>
    {
        public async Task Broadcast(ChatMessage message)
        {
            await Clients.All.MessageReceived(message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.NewUserConnected("A new user has joined the chat.");
        }
    }
}
