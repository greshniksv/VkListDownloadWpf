using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace VkListDownloader2.DTO
{
    public sealed class SkipedItem : INotifyPropertyChanged
    {
        private string name;
        private Uri url;

        public Uri Url
        {
            get { return url; }
            set
            {
                url = value;
                RaisePropertyChanged(nameof(url));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
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
