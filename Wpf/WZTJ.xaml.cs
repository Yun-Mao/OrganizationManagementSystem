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
    /// WZTJ.xaml 的交互逻辑
    /// </summary>
    public partial class WZTJ : Window
    {
        string name2;
        int edit=0;
        public WZTJ()
        {
            edit = 0;
            InitializeComponent();
        }
        public WZTJ(string name2)
        {
            this.name2 = name2;
            edit = 1;
            InitializeComponent();
            name.Text = name2;
            name.IsReadOnly = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (edit == 0)
            {
                string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                string insertsql = "INSERT INTO WZ(name,num,remain,detail) VALUES('" + name.Text + "','" + num.Text + "','" + num.Text + "','" + detail.Text + "')";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    try
                    {
                        SqlCommand cmd = new SqlCommand(insertsql);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                        name.Text = "";
                        num.Text = "";
                        detail.Text = "";
                        MessageBox.Show("提交成功！！");
                        conn.Close();

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("有误请检查！！" + ex);
                    }

                }
            }
            else if(edit==1)
            {
                string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                string selectsql = "Select * from WZ where name='" + name2 + "'";
                
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    try
                    {
                        int num_new;
                        int.TryParse(num.Text, out num_new);
                        SqlCommand cmd = new SqlCommand(selectsql);
                        cmd.Connection = conn;
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        int num2 = (int)reader["num"];
                        int remain = (int)reader["remain"];
                        remain += num_new - num2;
                        reader.Close();
                        string updatesql = "Update WZ set num='"+num_new+"',remain='"+remain+"' where name='" + name2 + "'";
                        cmd = new SqlCommand(updatesql);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                        //name.Text = "";
                        num.Text = "";
                        detail.Text = "";
                        MessageBox.Show("提交成功！！");
                        conn.Close();

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("有误请检查！！" + ex);
                    }

                }
            }
            
        }
    }
}
