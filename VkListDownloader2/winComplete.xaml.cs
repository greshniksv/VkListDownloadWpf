using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using VkListDownloader2.DTO;

namespace VkListDownloader2
{
    /// <summary>
    /// Interaction logic for winComplete.xaml
    /// </summary>
    public partial class WinComplete : Window
    {
        public Dictionary<string, Uri> Result { get; set; }
        private readonly ObservableCollection<SkipedItem> skipedItems = new ObservableCollection<SkipedItem>();

        public WinComplete()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in Result)
            {
                skipedItems.Add(new SkipedItem {Name = item.Key, Url = item.Value });
            }
            lvSkipedList.ItemsSource = skipedItems;
        }

        private void btnExportToHtml_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog {Filter = "HTML file (*.html)|*.html"};
            saveFileDialog.ShowDialog();
            using (TextWriter textWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
            {
                textWriter.WriteLine("<h1> Skiped files: <h1> <hr /> ");
                textWriter.WriteLine(
                    "<table border='1'> <tr><th style='width: 200px;'> Name </th><th style='width: 100px;'> Url </th><tr> ");

                foreach (var skipedItem in skipedItems)
                {
                    textWriter.WriteLine(
                        $" <tr><td> {skipedItem.Name} </td><td> <a href='{skipedItem.Url}' download='{skipedItem.Name}' > Download </a> </td></tr> ");
                }
                textWriter.WriteLine("</table>");
            }
        }
    }
}
