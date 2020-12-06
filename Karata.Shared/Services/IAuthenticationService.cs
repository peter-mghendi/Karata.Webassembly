using Karata.Shared.Models;
using System.ComponentModel;

namespace Karata.Shared.Services
{
    public interface IAuthenticationService : INotifyPropertyChanged
    {
        public LoginResult Authentication { set; }

        public string AccessToken { get; }

        public bool IsAuthenticated { get; }

        public string RefreshToken { get; }

        public string Role { get; }
    }
}
