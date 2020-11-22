using Karata.Shared.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Karata.Server.Infrastructure
{
    public interface IJwtAuthManager
    {
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
        Task<JwtAuthResult> GenerateTokensAsync(string email, Claim[] claims, DateTime now);
        Task<JwtAuthResult> RefreshAsync(string refreshToken, string accessToken, DateTime now);
    }
}