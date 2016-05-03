using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace VkListDownloader2
{
    public class LinkDownloader
    {
        private int _downloaded;
        private int _skiped;
        private readonly int _threadCount;
        private readonly Dictionary<string, string> _linkDictionary;
        private readonly string _workDirectory;
        private readonly int _retryCount;
        private readonly Dictionary<string, string> _skipedList = new Dictionary<string, string>();
        public readonly Dictionary<string, string> RaiseEventCatch = new Dictionary<string, string>();
        public event Action<string, int, int> Progress;
        public event Action<int, int, int> TotalProgress;
        public event Action<Dictionary<string, string>> Complete;
        
        public LinkDownloader(int threadCount, int retryCount,
                string workDirectory, Dictionary<string, string> linkDictionary)
        {
            _threadCount = threadCount;
            _linkDictionary = linkDictionary;
            _workDirectory = workDirectory;
            _retryCount = retryCount;
            var thread = new Thread(Work)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void Work()
        {
            var threads = new List<Thread>();
            int total = 0;
            foreach (var link in _linkDictionary)
            {

                var activeThread = threads.Count(x => x.IsAlive);
                while (activeThread >= _threadCount)
                {
                    Thread.Sleep(200);
                    activeThread = threads.Count(x => x.IsAlive);
                }

                threads.RemoveAll(x => !x.IsAlive);

                KeyValuePair<string, string> link1 = link;
                var thread = new Thread(() => Downloaded(link1.Key, link1.Value))
                {
                    IsBackground = true
                };
                thread.Start();
                threads.Add(thread);
                total++;
                TotalProgress?.Invoke(total * 100 / _linkDictionary.Count, _downloaded, _skiped);
            }
            var alives = threads.Where(x => x.IsAlive);
            foreach (var thread in alives)
            {
                thread.Join();
            }
            Complete?.Invoke(_skipedList);
        }

        private void Downloaded(string name, string url)
        {
            var fileId = Tools.GetMD5Hash(string.Concat(name, url));
            var tempFileName = Path.Combine(_workDirectory, $"{fileId}.mp3");
            for (int i = 0; i <= _retryCount; i++)
            {
                var wc = new WebClient();
                int retry = i;
                RizeEvent(fileId, retry, 0);
                wc.DownloadProgressChanged += (sender, args) =>
                {
                    RizeEvent(fileId, retry, (int)(args.BytesReceived * 100 / args.TotalBytesToReceive));
                };

                wc.DownloadFileCompleted += (sender, args) =>
                {
                    RizeEvent(fileId, retry, 100);
                    var newFile = Path.Combine(_workDirectory, $"{name}.mp3");
                    int tryCount = 0;
                    while (File.Exists(newFile))
                    {
                        tryCount++;
                        newFile = Path.Combine(_workDirectory, $"{name}{tryCount}.mp3");
                    }
                    File.Move(tempFileName, newFile);
                };
                try
                {
                    throw new Exception("aff");
                    wc.DownloadFile(url, tempFileName);
                    wc.DownloadFileAsync(new Uri(url), tempFileName);
                    _downloaded++;
                    return;
                }
                catch (Exception ex) { Debug.WriteLine($"Exception: {ex}"); }
            }
            _skipedList.Add(name, url);
            _skiped++;
        }

        private void RizeEvent(string id, int retry, int persent)
        {
            var item = RaiseEventCatch.FirstOrDefault(x => x.Key == id).Value;
            if (item != null && item == $"{retry}{persent}")
            {
                return;
            }
            Progress?.Invoke(id, retry, persent);
        }
    }
}
