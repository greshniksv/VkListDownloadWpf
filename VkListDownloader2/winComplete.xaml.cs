using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VkListDownloader2
{
    /// <summary>
    /// Interaction logic for winComplete.xaml
    /// </summary>
    public partial class winComplete : Window
    {
        public string Result
        {
            set
            {
                //richTextBox.AppendText(value);
            }
        }

        public winComplete()
        {
            InitializeComponent();
        }
    }
}
