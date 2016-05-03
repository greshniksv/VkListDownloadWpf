using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

	    public readonly ObservableCollection<DownloadItemInfo> Items = new ObservableCollection<DownloadItemInfo>();
        private Dictionary<string, string> _urlDictionary;
        private BackgroundWorker _worker;
        private string fileList;

        public MainWindow()
		{
            _downloadItems = new ObservableCollection<DownloadItemInfo>();
            InitializeComponent();
		}

        private void btnList_Click(object sender, RoutedEventArgs e)
        {
            if (!Tools.TestDir(txbFolder.Text))
            {
                MessageBox.Show("Error use output folder ");
            }

            var dialog = new OpenFileDialog
            {
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                LockControl(true);
                txbList.Text = dialog.FileName;
                fileList = txbList.Text;
                _worker = new BackgroundWorker()
                {
                    WorkerReportsProgress = true
                };
                _worker.DoWork += worker_DoWork;
                _worker.ProgressChanged += (o, args) => {
                    
                };
                _worker.RunWorkerCompleted += (o, args) => {
                    LoadToListView(_urlDictionary);
                    var linkDownloader = new LinkDownloader((int)slThead.Value, (int)slRetry.Value, txbFolder.Text, _urlDictionary);
                    linkDownloader.Progress += linkDownloader_Progress;
                    linkDownloader.TotalProgress += LinkDownloader_TotalProgress;
                    linkDownloader.Complete += LinkDownloader_Complete;
                };
                _worker.RunWorkerAsync();

            }
           
        }

        private void LinkDownloader_Complete(Dictionary<string, string> skipedList)
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

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var result = new Dictionary<string, string>();
            string fileData;
            using (TextReader reader = new StreamReader(fileList, Encoding.UTF8))
            {
                fileData = reader.ReadToEnd();
            }

            int position = 0;
            while (position < fileData.Length)
            {
                var artist = Tools.FixFileName(GetBlock(fileData, ref position));
                var title = Tools.FixFileName(GetBlock(fileData, ref position));
                var url = GetBlock(fileData, ref position);
                var key = string.Concat(artist, " - ", title);
                if (!result.ContainsKey(key))
                {
                    result.Add(string.Concat(artist, " - ", title), url.Replace("h=", "http://"));
                }
            }
            _urlDictionary = result;
        }

        private string GetBlock(string data, ref int position)
        {
            string charCount = string.Empty;
            for (int i = position; i <= data.Length; i++)
            {
                if (data[i] != '|')
                {
                    charCount += data[i];
                }
                else
                {
                    position = i + 1;
                    break;
                }
            }
            var charCountInt = Int32.Parse(charCount);
            string result = data.Substring(position, charCountInt);
            position += charCountInt;
            return result;
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

        private void LoadToListView(Dictionary<string, string> dictionary)
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DownloadItems.Add(new DownloadItemInfo
            {
                Key = "afsdfsdf",
                Name = Guid.NewGuid().ToString(),
                Retry = 0,
                Progress = 0
            });
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
	        txbList.IsReadOnly = locked;
	        txbFolder.IsReadOnly = locked;
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
