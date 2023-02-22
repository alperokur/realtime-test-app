using System.ComponentModel;

namespace Winux.ViewModels
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
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

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                RaisePropertyChange(nameof(FirstName));
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                RaisePropertyChange(nameof(LastName));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChange(nameof(Email));
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                RaisePropertyChange(nameof(Phone));
            }
        }

        private string _register;
        public string Register
        {
            get => _register;
            set
            {
                _register = value;
                RaisePropertyChange(nameof(Register));
            }
        }

        private string _haveAccount;
        public string HaveAccount
        {
            get => _haveAccount;
            set
            {
                _haveAccount = value;
                RaisePropertyChange(nameof(HaveAccount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}