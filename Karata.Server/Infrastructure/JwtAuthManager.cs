using Karata.Server.Models;
using Karata.Server.Services;
using Karata.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Karata.Server.Infrastructure
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public JwtAuthManager(JwtTokenConfig jwtTokenConfig, IRefreshTokenService refreshTokenService)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _refreshTokenService = refreshTokenService;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public async Task<JwtAuthResult> GenerateTokensAsync(string email, Claim[] claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = await _refreshTokenService.CreateTokenAsync(email, _jwtTokenConfig.RefreshTokenExpiration, now);

            return new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<JwtAuthResult> RefreshAsync(string refreshToken, string accessToken, DateTime now)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var email = principal.Identity.Name;
            var existingRefreshToken = await _refreshTokenService.GetTokenAsync(refreshToken);

            if (existingRefreshToken == null || existingRefreshToken.Email != email || existingRefreshToken.ExpireAt < now)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return await GenerateTokensAsync(email, principal.Claims.ToArray(), now); // need to recover the original claims
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }
    }
}
