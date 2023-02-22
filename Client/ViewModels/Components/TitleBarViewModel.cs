

using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Winux.ViewModels.Components
{
    internal class TitleBarViewModel : INotifyPropertyChanged
    {
        private Visibility _authVisibility = Visibility.Collapsed;
        public Visibility AuthVisibility
        {
            get => _authVisibility;
            set
            {
                _authVisibility = value;
                RaisePropertyChange(nameof(AuthVisibility));
            }
        }

        private string _settings;
        public string Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                RaisePropertyChange(nameof(Settings));
            }
        }

        private string _language;
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                RaisePropertyChange(nameof(Language));
            }
        }

        private string _english;
        public string English
        {
            get => _english;
            set
            {
                _english = value;
                RaisePropertyChange(nameof(English));
            }
        }

        private string _turkish;
        public string Turkish
        {
            get => _turkish;
            set
            {
                _turkish = value;
                RaisePropertyChange(nameof(Turkish));
            }
        }

        private string _about;
        public string About
        {
            get => _about;
            set
            {
                _about = value;
                RaisePropertyChange(nameof(About));
            }
        }

        private string _copyright;
        public string Copyright
        {
            get => _copyright;
            set
            {
                _copyright = value;
                RaisePropertyChange(nameof(Copyright));
            }
        }

        private string _close;
        public string Close
        {
            get => _close;
            set
            {
                _close = value;
                RaisePropertyChange(nameof(Close));
            }
        }

        private string _logout;
        public string Logout
        {
            get => _logout;
            set
            {
                _logout = value;
                RaisePropertyChange(nameof(Logout));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}