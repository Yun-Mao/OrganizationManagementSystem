using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Charts;
namespace Wpf
{
    /// <summary>
    /// TJZJ.xaml 的交互逻辑
    /// </summary>
    public partial class TJZJ : System.Windows.Window
    {
        public TJZJ()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("金额", typeof(double));
            dt.Columns.Add("性质", typeof(string));
            dt.Columns.Add("经手人", typeof(string));
            dt.Columns.Add("时间", typeof(string));
            dt.Columns.Add("详细说明", typeof(string));
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string date = ZJ.SelectedDate.ToString();
            string[] YM = Regex.Split(date, "/", RegexOptions.IgnoreCase);
            //MessageBox.Show(YM[0]);
            //string selectsql = "SELECT * from totalmoney";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    DataRow row;
                    SqlCommand cmd = new SqlCommand("select * from ZJ");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string temptime = (string)reader["time"];
                        string[] YM_temp = Regex.Split(temptime, "/", RegexOptions.IgnoreCase);
                        if(YM[0]==YM_temp[0]&&YM[1]==YM_temp[1])
                        {
                            row = dt.NewRow();
                            row["金额"] = (double)reader["money"];
                            row["性质"] = (string)reader["type"];
                            row["经手人"] = (string)reader["people"];
                            row["时间"] = (string)reader["time"];
                            row["详细说明"] = (string)reader["detail"];
                            dt.Rows.Add(row);
                        } 
                    }
                    reader.Close();
                    //cmd = new SqlCommand(selectsql);
                    //cmd.Connection = conn;
                    //reader = cmd.ExecuteReader();
                    //reader.Read();
                    //double m = (double)reader["money"];
                    //yu_e.Text = m.ToString() + "  元";
                    //创建Excel

                    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）
                    Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出

                    //excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示
                    //将数据导入到工作表的单元格
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        excelWS.Cells[1, i + 1] = dt.Columns[i].ToString();
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            excelWS.Cells[i + 2, j + 1] = dt.Rows[i][j].ToString();   //Excel单元格第一个从索引1开始
                        }
                    }

                    excelWB.SaveAs("D:\\CW\\"+YM[0]+YM[1]+"month.xlsx");  //将其进行保存到指定的路径
                    excelWB.Close();
                    excelApp.Quit();  //KillAllExcel(excelApp); 释放可能还没释放的进程
                    MessageBox.Show("导出成功！");
                }
                catch
                {

                }
                conn.Close();
            }
        }
        private List<string> strListx = new List<string>() {"一月","二月","三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月","十二月" };
        private List<string> strListy = new List<string>();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string date = ZJ_year.SelectedDate.ToString();
            string[] YM = Regex.Split(date, "/", RegexOptions.IgnoreCase);
            double[] M = {1,1,1,1,1,1,1,1,1,1,1,1};
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from ZJ where type='出'");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string temptime = (string)reader["time"];
                        string[] YM_temp = Regex.Split(temptime, "/", RegexOptions.IgnoreCase);
                        double money=(double)reader["money"];
                        if (YM[0] == YM_temp[0])
                        {
                            switch(YM_temp[1])
                            {
                                case "1":M[0] += money;break;
                                case "2": M[1] += money; break;
                                case "3": M[2] += money; break;
                                case "4": M[3] += money; break;
                                case "5": M[4] += money; break;
                                case "6": M[5] += money; break;
                                case "7": M[6] += money; break;
                                case "8": M[7] += money; break;
                                case "9": M[8] += money; break;
                                case "10": M[9] += money; break;
                                case "11": M[10] += money; break;
                                case "12": M[11] += money; break;
                            }
                         }
                    }
                    for(int i = 0; i < 12; ++i)
                    {
                        strListy.Add(M[i].ToString());
                        //MessageBox.Show(M[i].ToString());
                    }
                    reader.Close();
                }
                catch
                {

                }
                conn.Close();
            }
            Simon.Children.Clear();
            CreateChartPie(YM[0]+"年每月资金统计", strListx, strListy);
        }
       
        //饼状图：
        public void CreateChartPie(string name, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Visifire.Charts.Chart chart = new Visifire.Charts.Chart();
            //设置图标的宽度和高度
            chart.Width = 580;
            chart.Height = 380;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示
            //创建一个标题的对象
            Title title = new Title();
            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);
            //向图标添加标题
            chart.Titles.Add(title);
            //Axis yAxis = new Axis();
            ////设置图标中Y轴的最小值永远为0           
            //yAxis.AxisMinimum = 0;
            ////设置图表中Y轴的后缀          
            //yAxis.Suffix = "斤";
            //chart.AxesY.Add(yAxis);
            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.Pie;//柱状Stacked

            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];
                dataPoint.LegendText = "##" + valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                //添加一个点击事件        
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }
            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);
            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            Simon.Children.Add(gr);
        }
        #region 点击事件
        //点击事件
        void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DataPoint dp = sender as DataPoint;
            //MessageBox.Show(dp.YValue.ToString());
        }
        #endregion
    }
}
