using Karata.Server.Data;
using Karata.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Karata.Server.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ILogger<RefreshTokenService> _logger;
        private readonly KarataContext _context;

        public RefreshTokenService(ILogger<RefreshTokenService> logger, KarataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<RefreshToken> GetTokenAsync(string tokenString)
        {
            return await _context.RefreshTokens.SingleAsync(r => r.TokenString == tokenString);
        }

        public async Task<RefreshToken> CreateTokenAsync(string email, int expiry, DateTime now)
        {
            var refreshToken = new RefreshToken
            {
                Email = email,
                TokenString = await GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(expiry)
            };

            var entry = await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        // optional: clean up expired refresh tokens
        public async Task RemoveExpiredRefreshTokensAsync(DateTime now)
        {
            var expiredTokens = _context.RefreshTokens.Where(x => x.ExpireAt < now);
            var count = await expiredTokens.CountAsync();

            if (count > 0)
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
                _logger.LogInformation($"Removed [{count}] refresh tokens from database.");
                await _context.SaveChangesAsync();
            }
        }

        // can be more specific to ip, user agent, device name, etc.
        public async Task RemoveRefreshTokenByEmailAsync(string email)
        {
            var refreshTokens = _context.RefreshTokens.Where(x => x.Email == email);
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        private async Task<string> GenerateRefreshTokenString()
        {
            string tokenString;
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            do
            {
                randomNumberGenerator.GetBytes(randomNumber);
                tokenString = Convert.ToBase64String(randomNumber);
            } while (await _context.RefreshTokens.CountAsync(r => r.TokenString == tokenString) > 0);

            return tokenString;
        }
    }
}
