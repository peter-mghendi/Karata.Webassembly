using Flurl;
using Flurl.Http;
using Karata.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Shared.Client
{
    public partial class KarataClient : IKarataClient
    {
        public async Task ChatAsync(ChatMessage message, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("chat").PostJsonAsync(message, cancellationToken);
    }
}
