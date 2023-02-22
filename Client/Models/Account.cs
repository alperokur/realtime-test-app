using System;
using System.ComponentModel;

namespace Winux.Models
{
    public class Account : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get => _id;

            set
            {
                _id = value;
                RaisePropertyChange(nameof(Id));
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

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate;

            set
            {
                _createdDate = value;
                RaisePropertyChange(nameof(CreatedDate));
            }
        }

        private DateTime _updatedDate;
        public DateTime UpdatedDate
        {
            get => _updatedDate;

            set
            {
                _updatedDate = value;
                RaisePropertyChange(nameof(UpdatedDate));
            }
        }

        private DateTime _deletedDate;
        public DateTime DeletedDate
        {
            get => _deletedDate;

            set
            {
                _deletedDate = value;
                RaisePropertyChange(nameof(DeletedDate));
            }
        }

        private int _permission;
        public int Permission
        {
            get => _permission;

            set
            {
                _permission = value;
                RaisePropertyChange(nameof(Permission));
            }
        }


        private int _rateLimitWindowInMinutes;
        public int RateLimitWindowInMinutes
        {
            get => _rateLimitWindowInMinutes;

            set
            {
                _rateLimitWindowInMinutes = value;
                RaisePropertyChange(nameof(RateLimitWindowInMinutes));
            }
        }


        private int _permitLimit;
        public int PermitLimit
        {
            get => _permitLimit;

            set
            {
                _permitLimit = value;
                RaisePropertyChange(nameof(PermitLimit));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChange(
            string name) { 
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(name)); }
    }
}