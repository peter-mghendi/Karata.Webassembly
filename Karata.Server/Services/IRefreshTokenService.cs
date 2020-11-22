using Karata.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Karata.Server.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateTokenAsync(string email, int expiry, DateTime now);
        Task<RefreshToken> GetTokenAsync(string tokenString);
        Task RemoveExpiredRefreshTokensAsync(DateTime now);
        Task RemoveRefreshTokenByEmailAsync(string email);
    }
}