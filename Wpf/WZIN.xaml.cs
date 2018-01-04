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
    /// WZIN.xaml 的交互逻辑
    /// </summary>
    public partial class WZIN : Window
    {
        string name1;
        int remain1;
        string user;
        public WZIN()
        {
            InitializeComponent();
        }
        public WZIN(string name,int remain,string user)
        {
            
            this.name1 = name;
            this.remain1 = remain;
            InitializeComponent();
            this.name.Text = name1;
            this.remain.Text = remain.ToString();
            this.user = user;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string strYMD = DateTime.Now.ToString();
            int num2;
            int.TryParse(num.Text.ToString(), out num2);
            string insertsql = "INSERT INTO WZ_temp(name,num,touser,time,detail,state) VALUES('" + name1 + "','"+num2+"','" + user + "','" + strYMD + "','" + WZ_detail.Text + "','1')";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //try
                //{
                    SqlCommand cmd = new SqlCommand(insertsql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();

                    string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + user + "','大学生志愿服务中心','我向你发送了物资申请','" + strYMD + "','1')";
                    cmd = new SqlCommand(insertsql2, conn);
                    cmd.ExecuteNonQuery();
                    num.Text = "";
                    WZ_detail.Text = "";
                    MessageBox.Show("提交成功！！");
                    conn.Close();
               // }
                //catch (SqlException ex)
                //{
               //     MessageBox.Show("有误请检查！！" + ex);
               // }

            }
        }
    }
}
