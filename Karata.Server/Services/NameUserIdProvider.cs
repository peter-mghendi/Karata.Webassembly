using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Karata.Server.Services
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection) => connection.User?.FindFirst(ClaimTypes.Name)?.Value;
    }
}
