using Karata.Shared.Models;
using Karata.Shared.Services;

namespace Karata.Web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public LoginResult Authentication { private get; set; }

        public string Role => Authentication.Role;

        public string Token => Authentication.AccessToken;

        public bool IsAuthenticated => Authentication?.AccessToken is not null;

        public AuthenticationService() => Authentication = new();
    }
}
