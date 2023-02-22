using System.ComponentModel;
using Winux;

namespace Winux.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public LoginViewModel()
        {
            if (App.LocalSettings.Values["AvatarUri"] != null)
            {
                _avatarUri = (string)App.LocalSettings.Values["AvatarUri"];
            }

        }

        private string _avatarUri;
        public string AvatarUri
        {
            get => _avatarUri;
            set
            {
                _avatarUri = value;
                RaisePropertyChange(nameof(AvatarUri));
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChange(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChange(nameof(Password));
            }
        }

        private string _rememberMe;
        public string RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                RaisePropertyChange(nameof(RememberMe));
            }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                RaisePropertyChange(nameof(Login));
            }
        }

        private string _haventAccount;
        public string HaventAccount
        {
            get => _haventAccount;
            set
            {
                _haventAccount = value;
                RaisePropertyChange(nameof(HaventAccount));
            }
        }

        private string _info;
        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                RaisePropertyChange(nameof(Info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}