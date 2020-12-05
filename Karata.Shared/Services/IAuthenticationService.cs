using Karata.Shared.Models;
using System.ComponentModel;

namespace Karata.Shared.Services
{
    public interface IAuthenticationService : INotifyPropertyChanged
    {
        public LoginResult Authentication { set; }

        public bool IsAuthenticated { get; }

        public string Role { get; }

        public string Token { get; }
    }
}
