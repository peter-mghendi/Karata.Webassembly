using Karata.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Karata.Server.Infrastructure
{
    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<JwtRefreshTokenCache> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JwtRefreshTokenCache(ILogger<JwtRefreshTokenCache> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Refresh token cache cleanup service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var refreshTokenService = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
            await refreshTokenService.RemoveExpiredRefreshTokensAsync(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("Refresh token cache cleanup service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
