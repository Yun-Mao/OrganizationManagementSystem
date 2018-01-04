using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// ZJ.xaml 的交互逻辑
    /// </summary>
    public partial class ZJ : Window
    {
        string name;
        public ZJ()
        {
            InitializeComponent();
        }
        public ZJ(string name)
        {
            InitializeComponent();
            this.name = name;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQJE.Text = "";
            Text_Detail.Text = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DateTime currentTime = new DateTime();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string strYMD = DateTime.Now.ToString();
            string insertsql = "INSERT INTO ZJ_temp(money,type,people,time,detail) VALUES('" + SQJE.Text + "','出','" + name + "','"+strYMD+"','" + Text_Detail.Text + "')";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(insertsql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                   
                    string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','大学生志愿服务中心','我向你发送了资金申请','" + strYMD + "','1')";
                    cmd = new SqlCommand(insertsql2, conn);
                    cmd.ExecuteNonQuery();
                    SQJE.Text = "";
                    Text_Detail.Text = "";
                    MessageBox.Show("提交成功！！");
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("有误请检查！！"+ex);
                }

            }
        }
    }
}
