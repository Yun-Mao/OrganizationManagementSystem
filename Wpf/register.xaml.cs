using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// register.xaml 的交互逻辑
    /// </summary>
    public partial class register : Window
    {
        string filename= "";
        public register()
        {
            InitializeComponent();  
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Min(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_register(object sender, RoutedEventArgs e)
        {
            if(password.Password!=password2.Password)
            {
                error.Text = "两次密码不一致！！";
            }
            else if(username.Text==""||password.Password=="")
            {
                error.Text = "用户名或密码不能为空！！";
            }
            else
            {
                int isonline = 0;
                Byte[] btye2;
                if (filename!="")//读取图片
                {
                    FileStream fs = new FileStream(@filename, FileMode.Open, FileAccess.Read);
                    btye2 = new byte[fs.Length];
                    fs.Read(btye2, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }
                else
                {
                    btye2 = null;
                }
                string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                string power;
                if (zhu.IsChecked == true)//读取权限
                {
                    power = "10";
                }
                else
                {
                    power = "5";
                }
                string insertsql = "INSERT INTO login(name,passward,power,img,isonline) VALUES('" + username.Text + "','" + password.Password + "','" + power + "',@imgfile," + isonline + ")";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    try
                    {
                        SqlCommand cmd = new SqlCommand(insertsql);
                        cmd.Connection = conn;

                        SqlParameter par = new SqlParameter("@imgfile", SqlDbType.Image);
                        par.Value = btye2;
                        cmd.Parameters.Add(par);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        error.Text = "注册成功！！";
                    }
                    catch (SqlException ex)
                    {
                        error.Text = "用户名重复！！错误ID:" + ex;
                    }

                }
            }
        }

        private void button1_image(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "头像图片 (.jpg)|*.jpg";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                this.filename = dlg.FileName;
                FileStream fs = new FileStream(@filename, FileMode.Open, FileAccess.Read);
                Byte[] btye2 = new byte[fs.Length];
                fs.Read(btye2, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(btye2);
                bitmap.EndInit();
                Image1.ImageSource = bitmap;
            }
        }
    }
}
