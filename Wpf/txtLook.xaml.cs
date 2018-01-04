using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
namespace Wpf
{
    /// <summary>
    /// txtLook.xaml 的交互逻辑
    /// </summary>
    public partial class txtLook : Window
    {
        string filename;
        bool admin;
        public txtLook(string filename1,bool admin)
        {
            InitializeComponent();
            this.filename = filename1;
            this.admin = admin;
            cut.IsEnabled = admin;
            savefile.IsEnabled = admin;
            paste.IsEnabled = admin;
            copy.IsEnabled = admin;
            if (admin == false)
            {
                textfile.IsReadOnly = true;
            }
            string dirpath;
            dirpath = "D:\\CW\\doc\\" + filename;
            DirectoryInfo d = new DirectoryInfo(dirpath);
            if (d.Exists)
            {
                FileInfo[] fs = null;
                try { fs = d.GetFiles(); }
                catch { return; }
                foreach (FileInfo fil in fs)
                {
                    ComboBox_txtlook.Items.Add(new txtfile(fil.ToString()));
                }
            }
        }
        private void savefile_Click(object sender, RoutedEventArgs e)
        {
            string dir = "D:\\CW\\doc\\" + filename + "\\" + ComboBox_txtlook.SelectedValue.ToString();
            File.WriteAllText(dir, textfile.Text, Encoding.Default);
        }
        private void cut_Click(object sender, RoutedEventArgs e)
        {
            textfile.Cut();
        }
        private void copy_Click(object sender, RoutedEventArgs e)
        {
            textfile.Copy();
        }
        private void paste_Click(object sender, RoutedEventArgs e)
        {
            textfile.Paste();
        }

        private void txt_look_Click(object sender, RoutedEventArgs e)
        {
            string dir;
            dir = "D:\\CW\\doc\\" + filename +"\\"+ ComboBox_txtlook.SelectedValue.ToString();
            string dataFromFile = File.ReadAllText(dir, Encoding.Default);
            textfile.Text = dataFromFile;
        }
    }
}
