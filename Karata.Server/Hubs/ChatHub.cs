using Karata.Server.Hubs.Clients;
using Karata.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Karata.Server.Hubs
{
    public class ChatHub: Hub<IChatClient>
    {
        public async Task Broadcast(ChatMessage message)
        {
            Console.WriteLine($"Broadcasting message: {message.Content}.");
            await Clients.All.MessageReceived(message);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("A new user has joined the chat.");
            await Clients.All.NewUserConnected("A new user has joined the chat.");
        }
    }
}
