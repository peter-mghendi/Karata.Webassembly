using Karata.Shared.Models;
using Karata.Shared.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Karata.Web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private LoginResult _authentication;
        public LoginResult Authentication
        {
            private get => _authentication;
            set
            {
                _authentication = value;
                NotifyPropertyChanged();
            }
        }

        public string AccessToken => Authentication.AccessToken;

        public bool IsAuthenticated => Authentication?.AccessToken is not null;

        public string RefreshToken => Authentication.RefreshToken;

        public string Role => Authentication.Role;

        public string Username => Authentication.Username;

        public AuthenticationService() => Authentication = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}