using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace VkListDownloader2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

        private ObservableCollection<DownloadItemInfo> _downloadItems;
        public ObservableCollection<DownloadItemInfo> DownloadItems => _downloadItems;

	    public readonly ObservableCollection<DownloadItemInfo> Items = 
            new ObservableCollection<DownloadItemInfo>();

        public MainWindow()
		{
            _downloadItems = new ObservableCollection<DownloadItemInfo>();
            InitializeComponent();
		}

        private async void btnList_Click(object sender, RoutedEventArgs e)
        {
            string userName = txbUserName.Text;
            string password = txbPassword.Password;
            if (!Tools.TestDir(txbFolder.Text))
            {
                MessageBox.Show("Error use output folder ");
                return;
            }

            LockControl(true);

            Dictionary<string, Uri> audioList = await Task<Dictionary<string, Uri>>.Factory.StartNew(() =>
            {
                ApiGetter getter = new ApiGetter(userName, password);
                return getter.GetAudios();
            });

            LoadToListView(audioList);
            var linkDownloader = new LinkDownloader((int)slThead.Value, (int)slRetry.Value, txbFolder.Text, audioList);
            linkDownloader.Progress += linkDownloader_Progress;
            linkDownloader.TotalProgress += LinkDownloader_TotalProgress;
            linkDownloader.Complete += LinkDownloader_Complete;
        }

        private void LinkDownloader_Complete(Dictionary<string, Uri> skipedList)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                winComplete complete = new winComplete();
                StringBuilder skiped = new StringBuilder();
                foreach (var item in skipedList)
                {
                    skiped.AppendLine($"{item.Key} \t- {item.Value}");
                }
                complete.Result = skiped.ToString();
                complete.ShowDialog();
            });
        }

        private void LinkDownloader_TotalProgress(int totalProgress, int downloaded, int skiped)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                prgTotal.Value = totalProgress;
                lblDownloaded.Text = downloaded.ToString();
                lblSkiped.Text = skiped.ToString();
            });
        }
        
        void linkDownloader_Progress(string id, int retry, int progress)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var item = Items.FirstOrDefault(x => x.Key == id);
                if (item != null)
                {
                    item.Retry = retry;
                    item.Progress = progress;
                }
            });
        }

        private void LoadToListView(Dictionary<string, Uri> dictionary)
        {
            foreach (var item in dictionary)
            {
                Items.Add(new DownloadItemInfo
                {
                    Key = Tools.GetMD5Hash(string.Concat(item.Key, item.Value)),
                    Name = item.Key,
                    Retry = 0,
                    Progress = 0
                });
            }
            lvProgress.ItemsSource = Items;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

	    private void btnFolder_Click(object sender, RoutedEventArgs e)
	    {
            var dialog = new OpenFileDialog
            {
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result != null && result == true)
            {
                txbFolder.Text = dialog.FileName;
            }
        }

        private void slThead_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblThread.Content = $"Thread count: {(int)slThead.Value}";
        }

        private void slRetry_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblRetry.Content = $"Retry count: {(int)slRetry.Value}";
        }

	    private void LockControl(bool locked)
	    {
	        txbUserName.IsEnabled = !locked;
            txbPassword.IsEnabled = !locked;
            txbFolder.IsEnabled = !locked;
	        btnList.IsEnabled = !locked;
	        btnFolder.IsEnabled = !locked;
	        slRetry.IsEnabled = !locked;
	        slThead.IsEnabled = !locked;
	    }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            winInform infrom = new winInform();
            infrom.ShowDialog();
        }
    }
}
