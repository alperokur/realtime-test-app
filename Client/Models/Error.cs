using System.ComponentModel;

namespace Winux.Models
{
    public class Error : INotifyPropertyChanged
    {
        private string _type;
        public string Type
        {
            get => _type;

            set
            {
                _type = value;
                RaisePropertyChange(nameof(Type));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                RaisePropertyChange(nameof(Title));
            }
        }

        private int _status;
        public int Status
        {
            get => _status;

            set
            {
                _status = value;
                RaisePropertyChange(nameof(Status));
            }
        }

        private string _detail;
        public string Detail
        {
            get => _detail;

            set
            {
                _detail = value;
                RaisePropertyChange(nameof(Detail));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChange(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}