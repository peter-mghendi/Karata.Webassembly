using Karata.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karata.Server.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(Message message);
    }
}
