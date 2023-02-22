using System.ComponentModel;
using Windows.UI.Xaml;

namespace Winux.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            if (App.LocalSettings.Values["IsLogin"] != null)
                _isLogin = (bool)App.LocalSettings.Values["IsLogin"];
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChange(nameof(Title));
            }
        }

        private bool _isLogin;
        public bool IsLogin
        {
            get { return _isLogin; }
            set
            {
                _isLogin = value;
                RaisePropertyChange(nameof(IsLogin));
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

        private string _terminal;
        public string Terminal
        {
            get => _terminal;
            set
            {
                _terminal = value;
                RaisePropertyChange(nameof(Terminal));
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

        private Visibility _terminalVisibility = Visibility.Collapsed;
        public Visibility TerminalVisibility
        {
            get => _terminalVisibility;
            set
            {
                _terminalVisibility = value;
                RaisePropertyChange(nameof(TerminalVisibility));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}