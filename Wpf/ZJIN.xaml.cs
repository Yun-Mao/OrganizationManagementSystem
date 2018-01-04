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
    public partial class ZJIN : Window
    {
        string name;
        public ZJIN()
        {
            InitializeComponent();
        }
        public ZJIN(string name)
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
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string strYMD = DateTime.Now.ToString();
            string insertsql = "INSERT INTO ZJ(money,type,people,time,detail) VALUES('" + SQJE.Text + "','入','" + name + "','"+strYMD+"','" + Text_Detail.Text + "')";
            string selectsql = "SELECT * from totalmoney";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(insertsql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd= new SqlCommand(selectsql);
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    double money = (double)reader["money"];
                    money += double.Parse(SQJE.Text);
                    string updatetsql = "Update totalmoney set money ='" + money +"'";
                    reader.Close();
                    cmd = new SqlCommand(updatetsql);
                    cmd.Connection = conn;
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
