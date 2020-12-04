using Karata.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Shared.Client
{
    public interface IKarataClient
    {
        string BaseUrl { get; init; }

        Task<LoginResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task ChatAsync(ChatMessage message, CancellationToken cancellationToken = default);
        Task<UserDTO> DeleteSelfAsync(CancellationToken cancellationToken = default);
        Task<UserDTO> DeleteUserAsync(long id, CancellationToken cancellationToken = default);
        Task<UserDTO> FetchSelfAsync(CancellationToken cancellationToken = default);
        Task<UserDTO> FetchUserAsync(long id, CancellationToken cancellationToken = default);
        Task InvalidateTokenAsync(CancellationToken cancellationToken = default);
        Task<ICollection<UserDTO>> ListUsersAsync(CancellationToken cancellationToken = default);
        Task<LoginResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<UserDTO> RegisterUserAsync(SignupRequest request, CancellationToken cancellationToken = default);
        Task<LoginResult> StartImpersonationAsync(ImpersonationRequest request, CancellationToken cancellationToken = default);
        Task<LoginResult> StopImpersonationAsync(CancellationToken cancellationToken = default);
        Task UpdateSelfAsync(UserDTO userDTO, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(long id, UserDTO userDTO, CancellationToken cancellationToken = default);
    }
}