using Flurl;
using Flurl.Http;
using Karata.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Shared.Client
{
    public partial class KarataClient
    {
        public async Task<UserDTO> FetchSelfAsync(CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("me").GetJsonAsync<UserDTO>(cancellationToken);

        public async Task UpdateSelfAsync(UserDTO userDTO, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("me").PutJsonAsync(userDTO, cancellationToken);

        public async Task<UserDTO> DeleteSelfAsync(CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("me").DeleteAsync(cancellationToken).ReceiveJson<UserDTO>();
    }
}
