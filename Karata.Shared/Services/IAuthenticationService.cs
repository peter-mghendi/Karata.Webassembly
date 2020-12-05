using Karata.Shared.Models;

namespace Karata.Shared.Services
{
    public interface IAuthenticationService
    {
        public LoginResult Authentication { set; }

        public bool IsAuthenticated();

        public string Role { get; }

        public string Token { get; }
    }
}
