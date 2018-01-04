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
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.Win32;
using main;

namespace Wpf
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
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
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)//登录
        {
            try
            {
                string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                //查询密码是否正确和在线状态；
                string selectsql = "Select * from login where name='" + username.Text + "' and passward='" + password.Password + "'";
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    int online = (int)sdr["isonline"];
                    if (online == 1)//判断在线；
                    {
                        sdr.Close();
                        conn.Close();
                        error.Text = "该用户已在线！";

                    }
                    else
                    {
                        //设置在线；
                        sdr.Close();
                        string updatesql = "Update login set isonline = '1' where name='" + username.Text + "'";
                        cmd.Connection = conn;
                        cmd.CommandText = updatesql;
                        sdr = cmd.ExecuteReader();
                        sdr.Read();
                        sdr.Close();
                        error.Text = "登入成功！";

                        conn.Close();

                        //读取图片信息；
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from login where name='" + username.Text + "'";
                        SqlDataReader sdr2 = cmd.ExecuteReader();
                        sdr2.Read();
                        int power;
                        byte[] MyData = new byte[0];
                        MyData = (byte[])sdr2["img"];//读取第一个图片的位流
                        int ArraySize = MyData.GetUpperBound(0);//获得数据库中存储的位流数组的维度上限，用作读取流的上限
                        power = (int)sdr2["power"];//读取权限
                        FileStream fs = new FileStream(@"./CW/user/01.jpg", FileMode.Create, FileAccess.Write);
                        fs.Write(MyData, 0, ArraySize);
                        fs.Close();   //-- 写入到/CW/user/01.jpg;
                        conn.Close();
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(MyData);
                        bitmap.EndInit();
                        Image1.ImageSource = bitmap;
                        MainWindow main = new MainWindow(username.Text, power);
                        main.Show();
                        sdr2.Close();
                        this.Close();
                    }
                }
                else
                {
                    error.Text = "用户名或密码错误！或该用户已在线！";
                }
            }
            catch { }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//测试用的，不要删除
        {
            //将需要存储的图片读取为数据流
            FileStream fs = new FileStream(@"D:\imge.png", FileMode.Open, FileAccess.Read);
            Byte[] btye2 = new byte[fs.Length];
            fs.Read(btye2, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "update login set img=@imgfile where name='云昴'";
                SqlParameter par = new SqlParameter("@imgfile", SqlDbType.Image);
                par.Value = btye2;
                cmd.Parameters.Add(par);
                int t = (int)(cmd.ExecuteNonQuery());
                if (t > 0)
                {
                    Console.WriteLine("插入成功");
                }
                conn.Close();
            }
        }
        private void Button_Min(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TB_refister(object sender, RoutedEventArgs e)
        {

            register RE = new register();
            RE.Show();
            //this.Close();
        }
    }
}
