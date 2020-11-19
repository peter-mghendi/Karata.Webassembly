using Karata.Shared.Models;
using System.Threading.Tasks;

namespace Karata.Server.Hubs.Clients
{
    public interface IChatClient
    {
        Task MessageReceived(ChatMessage message);

        Task NewUserConnected(string message);
    }
}
