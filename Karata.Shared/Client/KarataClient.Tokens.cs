using Flurl;
using Flurl.Http;
using Karata.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Shared.Client
{
    public partial class KarataClient : IKarataClient
    {
        public async Task<LoginResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", "tokens")
                .PostJsonAsync(request, cancellationToken)
                .ReceiveJson<LoginResult>();

        public async Task InvalidateTokenAsync(CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", "tokens", "invalidate")
                .PostAsync(cancellationToken: cancellationToken);

        public async Task<LoginResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", "tokens", "refresh")
                .PostJsonAsync(request, cancellationToken)
                .ReceiveJson<LoginResult>();

        public async Task<LoginResult> StartImpersonationAsync(ImpersonationRequest request, CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", "tokens", "impersonation", "start")
                .PostJsonAsync(request, cancellationToken)
                .ReceiveJson<LoginResult>();

        public async Task<LoginResult> StopImpersonationAsync(CancellationToken cancellationToken = default) =>
            await BaseUrl.AppendPathSegments("users", "tokens", "impersonation", "stop")
                .PostAsync(cancellationToken: cancellationToken)
                .ReceiveJson<LoginResult>();
    }
}
