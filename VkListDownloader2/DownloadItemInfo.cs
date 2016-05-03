using System.ComponentModel;
using System.Runtime.CompilerServices;
using VkListDownloader2.Annotations;

namespace VkListDownloader2
{
    public class DownloadItemInfo : INotifyPropertyChanged
    {
        private string _key;
        private string _name;
        private int _retry;
        private int _progress;

        public string Key {
            get { return _key; }
            set
            {
                _key = value;
                RaisePropertyChanged(nameof(Key));
            }
        }
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public int Retry
        {
            get { return _retry; }
            set
            {
                _retry = value;
                RaisePropertyChanged(nameof(Retry));
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
