using System;
using System.ComponentModel;

namespace Winux.Models
{
    public class Token : INotifyPropertyChanged
    {

        private string _accessToken;
        public string AccessToken
        {
            get => _accessToken;

            set
            {
                _accessToken = value;
                RaisePropertyChange(nameof(AccessToken));
            }
        }

        private string _refreshToken;
        public string RefreshToken
        {
            get => _refreshToken;

            set
            {
                _refreshToken = value;
                RaisePropertyChange(nameof(RefreshToken));
            }
        }

        private DateTime _expiration;
        public DateTime Expiration
        {
            get => _expiration;

            set
            {
                _expiration = value;
                RaisePropertyChange(nameof(Expiration));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChange(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}