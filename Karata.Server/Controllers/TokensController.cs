using Karata.Server.Data;
using Karata.Server.Infrastructure;
using Karata.Server.Services;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Karata.Server.Controllers
{
    [Route("api/users/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly KarataContext _context;
        private readonly ILogger<TokensController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokensController(
            KarataContext context,
            ILogger<TokensController> logger,
            IUserService userService,
            IJwtAuthManager jwtAuthManager,
            IRefreshTokenService refreshTokenService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _refreshTokenService = refreshTokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!await _userService.IsValidUserCredentialsAsync(request.Email, request.Password))
            {
                return NotFound();
            }

            var role = await _userService.GetUserRoleAsync(request.Email);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, request.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = await _jwtAuthManager.GenerateTokensAsync(request.Email, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.Email}] logged in the system.");

            return new LoginResult
            {
                Email = request.Email,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        [HttpPost("invalidate")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var email = User.Identity.Name;
            await _refreshTokenService.RemoveRefreshTokenByEmailAsync(email); // can be more specific to ip, user agent, device name, etc. 
            _logger.LogInformation($"User [{email}] logged out the system.");
            return Ok();
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<LoginResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var email = User.Identity.Name;
                _logger.LogInformation($"User [{email}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return BadRequest();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = await _jwtAuthManager.RefreshAsync(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{email}] has refreshed JWT token.");
                return new LoginResult
                {
                    Email = email,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                };
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        [HttpPost("impersonation/start")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<LoginResult>> Impersonate([FromBody] ImpersonationRequest request)
        {
            var email = User.Identity.Name;
            _logger.LogInformation($"User [{email}] is trying to impersonate [{request.Email}].");

            var impersonatedRole = await _userService.GetUserRoleAsync(request.Email);
            if (string.IsNullOrWhiteSpace(impersonatedRole))
            {
                _logger.LogInformation($"User [{email}] failed to impersonate [{request.Email}] due to the target user not found.");
                return BadRequest($"The target user [{request.Email}] is not found.");
            }
            if (impersonatedRole == Policies.Admin)
            {
                _logger.LogInformation($"User [{email}] is not allowed to impersonate another Admin.");
                return BadRequest("This action is not supported.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.Email),
                new Claim(ClaimTypes.Role, impersonatedRole),
                new Claim("OriginalEmail", email)
            };

            var jwtResult = await _jwtAuthManager.GenerateTokensAsync(request.Email, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.Email}] is impersonating [{request.Email}] in the system.");

            return new LoginResult
            {
                Email = request.Email,
                Role = impersonatedRole,
                OriginalEmail = email,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        [HttpPost("impersonation/stop")]
        [Authorize]
        public async Task<ActionResult<LoginResult>> StopImpersonation()
        {
            var email = User.Identity.Name;
            var originalEmail = User.FindFirst("OriginalEmail")?.Value;
            if (string.IsNullOrWhiteSpace(originalEmail))
            {
                return BadRequest("You are not impersonating anyone.");
            }
            _logger.LogInformation($"User [{originalEmail}] is trying to stop impersonate [{email}].");

            var role = await _userService.GetUserRoleAsync(originalEmail);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, originalEmail),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = await _jwtAuthManager.GenerateTokensAsync(originalEmail, claims, DateTime.Now);
            _logger.LogInformation($"User [{originalEmail}] has stopped impersonation.");

            return new LoginResult
            {
                Email = originalEmail,
                Role = role,
                OriginalEmail = null,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }
    }
}
