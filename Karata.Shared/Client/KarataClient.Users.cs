using Flurl;
using Flurl.Http;
using Karata.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Shared.Client
{
    public partial class KarataClient
    {
        public async Task<ICollection<UserDTO>> ListUsersAsync(CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("users").GetJsonAsync<ICollection<UserDTO>>(cancellationToken);

        public async Task<UserDTO> RegisterUserAsync(SignupRequest request, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegment("users").PostJsonAsync(request, cancellationToken).ReceiveJson<UserDTO>();

        public async Task<UserDTO> FetchUserAsync(long id, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", id).GetJsonAsync<UserDTO>(cancellationToken);

        public async Task UpdateUserAsync(long id, UserDTO userDTO, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", id).PutJsonAsync(userDTO, cancellationToken);

        public async Task<UserDTO> DeleteUserAsync(long id, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", id).DeleteAsync(cancellationToken).ReceiveJson<UserDTO>();
    }
}
