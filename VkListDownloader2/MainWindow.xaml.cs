using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using VkListDownloader2.DTO;
using VkNet.Exception;

namespace VkListDownloader2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

	    private readonly ObservableCollection<DownloadItemInfo> downloadedItems = 
            new ObservableCollection<DownloadItemInfo>();

        public MainWindow()
		{
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

           

            Dictionary<string, Uri> audioList = await Task<Dictionary<string, Uri>>.Factory.StartNew(() =>
            {
                try
                {
                    ApiGetter getter = new ApiGetter(userName, password);
                    return getter.GetAudios();
                }
                catch (VkApiAuthorizationException)
                {
                    MessageBox.Show("Invalid authorization. Please check your email and password.");
                }
                return null;
            });

            if (audioList == null)
            {
                return;
            }

            LockControl(true);
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
                WinComplete complete = new WinComplete {Result = skipedList};
                complete.ShowDialog();
                LockControl(false);
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
                var item = downloadedItems.FirstOrDefault(x => x.Key == id);
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
                downloadedItems.Add(new DownloadItemInfo
                {
                    Key = Tools.GetMD5Hash(string.Concat(item.Key, item.Value)),
                    Name = item.Key,
                    Retry = 0,
                    Progress = 0
                });
            }
            lvProgress.ItemsSource = downloadedItems;
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
