using System.ComponentModel;
using System.Runtime.CompilerServices;
using VkListDownloader2.Annotations;

namespace VkListDownloader2.DTO
{
    public sealed class DownloadItemInfo : INotifyPropertyChanged
    {
        private string key;
        private string name;
        private int retry;
        private int progress;

        public string Key {
            get { return key; }
            set
            {
                key = value;
                RaisePropertyChanged(nameof(Key));
            }
        }
        public string Name {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public int Retry
        {
            get { return retry; }
            set
            {
                retry = value;
                RaisePropertyChanged(nameof(Retry));
            }
        }

        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
