using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Wpf;


namespace main
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public class Filestr
    {
        public string Name { set; get; }
        public int EmpID { set; get; }
    }


    public partial class MainWindow : System.Windows.Window
    {
        private ObservableCollection<DataInfo> dataInfos = new ObservableCollection<DataInfo>();//用户信息
        Socket sokConnection = null;
        Socket socketWatch = null;//服务器端socket
        Socket sockClient = null;//客户端socket
        Thread threadWatch = null;//服务器端监听
        Thread threadClient = null;//客户端线程
        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();
        string s1 = "";
        bool socketflag = true;
        //int filecount = 0;
        string filename;
        IndexList l = new IndexList();
        Purer p = new Purer();
        string name;
        bool power;
        string iptemp;
        //Thread TReadIndex;
        public MainWindow()
        {
            InitializeComponent();
            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

        }
        public MainWindow(string name, int power)
        {
            InitializeComponent();
            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();
            this.name = name;
            buttonEnd3.Visibility = Visibility.Collapsed;
            if (power == 10)
            {
                this.power = true;
                buttonStart3.Visibility = Visibility.Collapsed;
                buttonSend.Visibility = Visibility.Collapsed;
                filesend.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.power = false;
                KQQHY.Visibility = Visibility.Collapsed;
                buttonSend2.Visibility = Visibility.Collapsed;
                CKSQ.Visibility = Visibility.Collapsed;
                TJnewWZ.Visibility = Visibility.Collapsed;
                BHLY1.Visibility = Visibility.Collapsed;
                BH2.Visibility = Visibility.Collapsed;
                TY2.Visibility = Visibility.Collapsed;
                GJ.Visibility = Visibility.Collapsed;
                CKSQ2.Visibility = Visibility.Collapsed;
                TJZJ.Visibility = Visibility.Collapsed;
                TIANJZJ.Visibility = Visibility.Collapsed;
                BHLY.Visibility = Visibility.Collapsed;
                bh.Visibility = Visibility.Collapsed;
                ty.Visibility = Visibility.Collapsed;
                sc.Visibility = Visibility.Collapsed;
                JSQHY.Visibility = Visibility.Collapsed;
                DCWZ.Visibility = Visibility.Collapsed;
            }
            username.Text = "欢迎您，" + name;
            ComboBox1.Items.Add(new Filestr { EmpID = 1, Name = "总则" });
            ComboBox1.Items.Add(new Filestr { EmpID = 2, Name = "办公室工作手册" });
            ComboBox1.Items.Add(new Filestr { EmpID = 3, Name = "培训部工作手册" });
            ComboBox1.Items.Add(new Filestr { EmpID = 4, Name = "外联部工作手册" });
            ComboBox1.Items.Add(new Filestr { EmpID = 5, Name = "监察部工作手册" });
            Image1.ImageSource = new BitmapImage(new Uri("CW\\user\\01.jpg", UriKind.Relative));
            LoadData();
            //LoadData_WZ();
            listView_first.Items.Clear();
            string selectsql = "Select * from Message where touser='" + name + "' and state='1'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string fromuser = reader["fromuser"].ToString();
                    string time = reader["time"].ToString();
                    int state = (int)reader["state"];
                    string text = reader["message"].ToString();
                    listView_first.Items.Add(new message(fromuser, time, text, state));
                }
            }
        }
        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void WMButton_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                SqlConnection conn = new SqlConnection(str);
                SqlCommand cmd = new SqlCommand();
                string updatesql = "Update login set isonline = '0' where name='" + name + "'";
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = updatesql;
                cmd.ExecuteReader();
                conn.Close();
                this.Close();
            }
            catch { }

        }

        private void WMButton_Click_5(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void WMButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            test.Text = ICTCLAS.Test("110");
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)//主界面按钮，测试功能
        {
            string Ip;
            string dk;
            using (StreamReader sr = new StreamReader("D:/CW/config"))
            {
                Ip = sr.ReadLine();
                dk = sr.ReadLine();
            }
            IPAddress ip = IPAddress.Parse(Ip);
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(dk));
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ShowMsg(information, "与服务器连接中。。。 。。。");
                sockClient.Connect(endPoint);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                return;
            }
            ShowMsg(information, "与服务器链接成功！！！");

        }

        private void B_search_Click(object sender, RoutedEventArgs e)
        {
            filename = ComboBox1.SelectedValue.ToString();
            if (searchcon.Text == "")
            {
                MessageBox.Show("请输入你要搜索的关键字！");
                return;
            }
            error.Text = "正在搜索";
            B_search.IsEnabled = false;
            s1 = searchcon.Text;
            listView.Items.Clear();
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
                    search(fil.FullName);

                }
            }
            B_search.IsEnabled = true;
            error.Text = "搜索完毕";
        }
        public void search(string file)
        {
            FileStream f = null;
            try { f = new FileStream(file, FileMode.Open, FileAccess.Read); }
            catch { return; }
            StreamReader r = new StreamReader(f, Encoding.GetEncoding("gb2312"));
            string xxx = "";
            int line = 0, ch = 0, index = 0;
            while (r.Peek() != 1)
            {
                xxx = r.ReadLine();
                index = 0;
                line++;
                if (xxx == null)
                {
                    break;
                }
                while (xxx.IndexOf(s1, index) != -1)
                {
                    ch = xxx.IndexOf(s1, index) + 1;
                    index += ch;
                    if (index > xxx.Length)
                    {
                        break;
                    }
                    addlist(file, line.ToString(), ch.ToString());
                }

            }
            r.Close();
            f.Close();
        }
        void addlist(string name, string line, string ch)
        {
            string fname = name.Substring(name.LastIndexOf("\\") + 1, name.Length - name.LastIndexOf("\\") - 1);
            string[] a = { fname, line, ch, name };

            listView.Items.Add(new Filename(fname, line, ch, name));
        }
        void openfile(string filename)
        {
            try
            {
                string syspath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                ProcessStartInfo n = new ProcessStartInfo();
                n.FileName = syspath + "\\notepad.exe";
                n.Arguments = filename;
                Process.Start(n);
            }
            catch
            {

            }
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object o = listView.SelectedItem;
            if (o == null)
                return;
            Filename a = o as Filename;
            PositionNotepad p = new PositionNotepad();
            //openfile(a.Name.ToString());
            bool res = PositionNotepad.PositionNotePad(a.Name.ToString(), a.Line.ToString());
        }
        private void B_look_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                filename = ComboBox1.SelectedValue.ToString();
                txtLook txt = new txtLook(filename, power);
                txt.Show();
            }
            catch
            {

            }

        }

        private void B_edict_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonStart2_Click(object sender, RoutedEventArgs e)
        {
            KQQHY.Visibility = Visibility.Collapsed;
            JSQHY.Visibility = Visibility.Visible;
            socketflag = true;
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(IP.Text.ToString());
            IPEndPoint endPoint = new IPEndPoint(address, int.Parse(DK.Text.ToString()));

            try
            {
                socketWatch.Bind(endPoint);
            }
            catch (SocketException se)
            {
                MessageBox.Show("异常：" + se.Message);
                return;
            }

            socketWatch.Listen(10);
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            ShowMsg(information, "等待成员进入!");
        }
        void WatchConnecting()
        {
            while (socketflag)
            {
                sokConnection = socketWatch.Accept();
                ListBoxSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                //workchart wc = new workchart(sokConnection.RemoteEndPoint.ToString());
                //ListBoxSend.Items.Add(new workchart("hehe",sokConnection.RemoteEndPoint.ToString()));
            });
                iptemp = sokConnection.RemoteEndPoint.ToString();
                dict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                ShowMsg(information, "客户端链接成功！");
                Thread thr = new Thread(RecMsg);
                thr.IsBackground = true;
                thr.Start(sokConnection);
                dictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);
            }
        }
        void ShowMsg(RichTextBox txt, string str)
        {
            txt.Dispatcher.Invoke(
            DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                txt.AppendText(str);
            });
        }
        void RecMsg(object sokConnectionparn)//服务器监听
        {
            Socket sokClient = sokConnectionparn as Socket;
            while (true)
            {
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                int length = -1;
                try
                {
                    length = sokClient.Receive(arrMsgRec);
                }
                catch (Exception se)
                {
                    ShowMsg(information, "异常：" + se.Message);
                    //dict.Remove(sokClient.RemoteEndPoint.ToString());
                    //dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    //ListBoxSend.Items.Remove(sokClient.RemoteEndPoint.ToString());

                    break;
                }
                if (arrMsgRec[0] == 0)//表示接收到的是消息文件
                {
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                    ShowMsg(richTextBoxMain, strMsg);
                    byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                    byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                    arrSendMsg[0] = 0;
                    Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                    foreach (Socket s in dict.Values)
                    {
                        s.Send(arrSendMsg);
                    }
                }
                if (arrMsgRec[0] == 1) // 表示接收到的是文件数据；  
                {

                    try
                    {
                        Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                        Nullable<bool> result = sfd.ShowDialog();
                        if (result == true)
                        {
                            string fileSavePath = sfd.FileName;// 获得文件保存的路径；  
                                                               // 创建文件流，然后根据路径创建文件；  
                            using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                            {
                                fs.Write(arrMsgRec, 1, length - 1);
                                ShowMsg(richTextBoxMain, "文件保存成功：" + fileSavePath);
                            }
                        }
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }
                }
                if (arrMsgRec[0] == 2) // 表示接收到的是用户信息；  
                {

                    try
                    {
                        string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                        ListBoxSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                ListBoxSend.Items.Add(new workchart(strMsg, iptemp));
            });
                        Thread.Sleep(1000);
                        byte[] arrSendMsg1 = new byte[1];
                        arrSendMsg1[0] = 3;
                        foreach (Socket s in dict.Values)
                        {
                            s.Send(arrSendMsg1);
                        }
                        Thread.Sleep(1000);
                        for (int i = 0; i < ListBoxSend.Items.Count; i++)
                        {
                            workchart item = (workchart)ListBoxSend.Items[i];
                            string strtemp = item.Name + "##" + item.IP;
                            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strtemp);
                            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                            arrSendMsg[0] = 2;
                            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                            foreach (Socket s in dict.Values)
                            {
                                s.Send(arrSendMsg);
                            }
                        }

                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }

                }
                if (arrMsgRec[0] == 4) // 表示接收到的是私法数据；
                {
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);// 将接受到的字节数据转化成字符串；
                    string[] sArray = Regex.Split(strMsg, "###", RegexOptions.IgnoreCase);
                    Socket s = dict[sArray[0]];
                    byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(sArray[1]);
                    byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                    arrSendMsg[0] = 0;
                    Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                    s.Send(arrSendMsg);
                    //ShowMsg(richTextBoxMain, strMsg);
                }
            }
        }

        private void buttonStart3_Click(object sender, RoutedEventArgs e)//客户端开启
        {
            buttonStart3.Visibility = Visibility.Collapsed;
            IPAddress ip = IPAddress.Parse(IP.Text.ToString());
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(DK.Text.ToString()));
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ShowMsg(information, "与服务器连接中。。。 。。。");
                sockClient.Connect(endPoint);
                string strMsg = name;
                byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                arrSendMsg[0] = 2;
                Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                sockClient.Send(arrSendMsg);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                return;
            }
            ShowMsg(information, "与服务器链接成功！！！");

            threadClient = new Thread(RecMsg2);
            threadClient.IsBackground = true;
            threadClient.Start();
        }
        void RecMsg2()
        {
            while (true)
            {
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                int length = -1;
                try
                {
                    length = sockClient.Receive(arrMsgRec);
                }
                catch (Exception e)
                {
                    ShowMsg(information, "异常：" + e.Message);
                    return;
                }
                if (arrMsgRec[0] == 0) // 表示接收到的是消息数据；
                {
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);// 将接受到的字节数据转化成字符串；
                    ShowMsg(richTextBoxMain, strMsg);
                }
                if (arrMsgRec[0] == 1) // 表示接收到的是文件数据；  
                {

                    try
                    {
                        Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                        Nullable<bool> result = sfd.ShowDialog();
                        if (result == true)
                        {
                            string fileSavePath = sfd.FileName;// 获得文件保存的路径；  
                                                               // 创建文件流，然后根据路径创建文件；  
                            using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                            {
                                fs.Write(arrMsgRec, 1, length - 1);
                                ShowMsg(richTextBoxMain, "文件保存成功：" + fileSavePath);
                            }
                        }
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }

                }
                if (arrMsgRec[0] == 2) // 表示接收到的是用户信息；  
                {

                    try
                    {
                        string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                        string[] sArray = Regex.Split(strMsg, "##", RegexOptions.IgnoreCase);
                        ListBoxSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                ListBoxSend.Items.Add(new workchart(sArray[0], sArray[1]));
            });
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }
                }
                if (arrMsgRec[0] == 3) // 表示接收到的是刷新用户信息；  
                {

                    try
                    {
                        ListBoxSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            { ListBoxSend.Items.Clear(); });
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }
                }
                if (arrMsgRec[0] == 9) // 表示接收到的是结束数据；
                {
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);// 将接受到的字节数据转化成字符串；
                    buttonSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                buttonSend.Visibility = Visibility.Collapsed;
            });
                    filesend.Dispatcher.Invoke(
                     DispatcherPriority.Normal,
             (ThreadStart)delegate
             {
                 filesend.Visibility = Visibility.Collapsed;
             });
                    ShowMsg(richTextBoxMain, strMsg);
                }
            }
        }
        private void button1_file(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                this.filename = dlg.FileName;
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    string fileName = System.IO.Path.GetFileName(filename);//文件的名字+扩展名，方便后面的保存操作；  
                    string fileExtension = System.IO.Path.GetExtension(filename);
                    string strMsg = "我给你发送的文件为： " + fileName + "\r\n";
                    byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                    byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                    arrSendMsg[0] = 0; // 用来表示发送的是消息数据  
                    Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                    sockClient.Send(arrSendMsg); // 发送消息；  

                    byte[] arrFile = new byte[1024 * 1024 * 2];
                    int length = fs.Read(arrFile, 0, arrFile.Length);  // 将文件中的数据读到arrFile数组中；  
                    byte[] arrFileSend = new byte[length + 1];
                    arrFileSend[0] = 1; // 用来表示发送的是文件数据；  
                    Buffer.BlockCopy(arrFile, 0, arrFileSend, 1, length);
                    sockClient.Send(arrFileSend);// 发送数据到服务端；  
                }
            }
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)//部门
        {
            if (SF.IsChecked == true)
            {
                string IP = ListBoxSend.SelectedValue.ToString();
                workchart WC = (workchart)ListBoxSend.SelectedItem;

                string i = name + "(私戳" + WC.Name + "):" + textBoxSend.Text.Trim() + "\r\n";
                string strMsg = IP + "###" + name + "(私戳你):" + textBoxSend.Text.Trim() + "\r\n";
                byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                arrSendMsg[0] = 4;
                Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                sockClient.Send(arrSendMsg);
                ShowMsg(richTextBoxMain, i);
                textBoxSend.Clear();
            }
            else
            {
                string strMsg = name + ":" + textBoxSend.Text.Trim() + "\r\n";
                byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                arrSendMsg[0] = 0;
                Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                sockClient.Send(arrSendMsg);
                //ShowMsg(richTextBoxMain,strMsg);
                textBoxSend.Clear();
            }

        }

        private void buttonSend2_Click(object sender, RoutedEventArgs e)
        {
            string strMsg = name + ":" + textBoxSend.Text.Trim() + "\r\n";
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
            arrSendMsg[0] = 0;
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            foreach (Socket s in dict.Values)
            {
                s.Send(arrSendMsg);
            }
            ShowMsg(richTextBoxMain, strMsg);
            textBoxSend.Clear();
        }

        private void buttonEnd3_Click(object sender, RoutedEventArgs e)
        {

        }
        public void SaveFile2(string filePath, string content)
        {

            try
            {
                if (File.Exists(filePath))
                {
                    System.Windows.Forms.DialogResult RESULT = System.Windows.Forms.MessageBox.Show("是否确认覆盖原有文件?", "信息提示", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (RESULT.ToString().Equals("Yes"))
                    {
                        File.Delete(filePath);
                        File.AppendAllText(filePath, content, Encoding.Default);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    File.AppendAllText(filePath, content, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void buttonEnd2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strMsg = "会议已结束\r\n";
                byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                arrSendMsg[0] = 9;
                Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                foreach (Socket s in dict.Values)
                {
                    s.Send(arrSendMsg);
                }
                ShowMsg(richTextBoxMain, strMsg);

                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                Nullable<bool> result = sfd.ShowDialog();
                if (result == true)
                {
                    string fileSavePath = sfd.FileName;// 获得文件保存的路径；  
                                                       // 创建文件流，然后根据路径创建文件；  
                    using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                    {
                        TextRange textRange = new TextRange(richTextBoxMain.Document.ContentStart, richTextBoxMain.Document.ContentEnd);
                        byte[] arrMsg2 = System.Text.Encoding.Default.GetBytes(textRange.Text);
                        fs.Write(arrMsg2, 0, arrMsg2.Length);
                        //SaveFile2(fileSavePath, richTextBoxMain.Document.ToString());
                        ShowMsg(richTextBoxMain, "聊天记录保存成功：" + fileSavePath);
                    }
                }
                buttonSend2.Visibility = Visibility.Collapsed;
                JSQHY.Visibility = Visibility.Collapsed;
                sokConnection.Close();
                socketWatch.Shutdown(SocketShutdown.Both);        
            }
            catch
            {

            }

        }

        private void MakeIdex_Click(object sender, RoutedEventArgs e)
        {
            string dirpath = test.Text;
            //当文件夹路径不为空的时候
            if (dirpath != "")
            {
                //目录
                System.IO.DirectoryInfo dirInfo = new DirectoryInfo(dirpath);
                //目录下文件
                FileInfo[] fi = dirInfo.GetFiles();
                //记录文件中最大词频
                List<int> FileMax = new List<int>();
                int filedone = 0;
                //一次分析文件夹中所有文件
                for (int i = 0; i < fi.Length; i++)
                {
                    //生成文件绝对路径
                    string filepath = dirpath;
                    if (!filepath.EndsWith("\\"))
                        filepath += "\\";
                    filepath += fi[i].Name;

                    //获取文件中的字符串
                    string content = "";
                    p.getTitleAnchorContent(filepath, ref content);
                    string[] sArray = Regex.Split(content, "\n", RegexOptions.IgnoreCase);
                    int num = sArray.Length;
                    int k;
                    for (k = 0; k < num; ++k)
                    {
                        //sArray[k] = p.findChinese(sArray[k]);
                        sArray[k] = ICTCLAS.Test(sArray[k]);
                        l.addContents(filedone, sArray[k], k + 1);
                    }
                    filedone++;
                    l.setIndexedFile(filedone);
                }
                l.Sort();
                l.writeIndexFile(dirpath);
                MessageBox.Show("完成！！");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ZJ zj = new ZJ(name);
            zj.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            listView_4.Items.Clear();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from ZJ_temp");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        double money1 = (double)reader["money"];
                        string type = (string)reader["type"];
                        string people = (string)reader["people"];
                        string time = (string)reader["time"];
                        string detail = (string)reader["detail"];
                        float money = (float)money1;
                        ZJ_Class zJ_Class = new ZJ_Class(money, type, people, time, detail);
                        listView_4.Items.Add(zJ_Class);
                    }
                }
                catch
                {

                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string strYMD = DateTime.Now.ToString();
            object o = listView_4.SelectedItem;
            if (o == null)
                return;
            ZJ_Class a = o as ZJ_Class;
            listView_4.Items.Remove(o);
            string deletesql = "DELETE FROM ZJ_temp WHERE money='" + a.Money + "' and people='" + a.People + "' and detail='" + a.Detail + "' and time='" + a.Time + "'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    if (BHLY.Text == "请填写驳回的理由：")
                    {
                        MessageBox.Show("请填写驳回的理由：");
                        return;
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand(deletesql);
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                        string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','" + a.People + "','我向你发送了资金申请驳回，理由为：" + BHLY.Text + "','" + strYMD + "','1')";
                        cmd = new SqlCommand(insertsql2, conn);
                        cmd.ExecuteNonQuery();
                        BHLY.Text = "请填写驳回的理由：";
                    }

                }
                catch
                {

                }
                conn.Close();
            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            object o = listView_4.SelectedItem;
            if (o == null)
                return;
            ZJ_Class a = o as ZJ_Class;
            string deletesql = "insert into ZJ select * from ZJ_temp WHERE money='" + a.Money + "' and people='" + a.People + "' and detail='" + a.Detail + "'" + " DELETE FROM ZJ_temp WHERE money='" + a.Money + "' and people='" + a.People + "' and detail='" + a.Detail + "'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string selectsql = "SELECT * from totalmoney";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(deletesql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(selectsql);
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    double money = (double)reader["money"];
                    money -= a.Money;
                    string updatetsql = "Update totalmoney set money ='" + money + "'";
                    reader.Close();
                    cmd = new SqlCommand(updatetsql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                catch
                {

                }
                conn.Close();
            }

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            listView_4.Items.Clear();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            string selectsql = "SELECT * from totalmoney";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from ZJ");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        double money1 = (double)reader["money"];
                        string type = (string)reader["type"];
                        string people = (string)reader["people"];
                        string time = (string)reader["time"];
                        string detail = (string)reader["detail"];
                        float money = (float)money1;
                        ZJ_Class zJ_Class = new ZJ_Class(money, type, people, time, detail);
                        listView_4.Items.Add(zJ_Class);
                    }
                    reader.Close();
                    cmd = new SqlCommand(selectsql);
                    cmd.Connection = conn;
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    double m = (double)reader["money"];
                    yu_e.Text = m.ToString() + "  元";

                }
                catch
                {

                }
                conn.Close();
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)//资金添加
        {
            ZJIN zjin = new ZJIN(name);
            zjin.Show();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            object o = listView_4.SelectedItem;
            if (o == null)
                return;
            ZJ_Class a = o as ZJ_Class;
            string deletesql = "DELETE FROM ZJ WHERE money='" + a.Money + "' and people='" + a.People + "' and detail='" + a.Detail + "' and time='" + a.Time + "'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(deletesql);
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                catch
                {

                }
                conn.Close();
            }
        }

        /// <summary>
        /// 发消息功能；
        /// </summary>
        private void LoadData()
        {
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from login");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataInfo model = new DataInfo();
                        string name = reader["name"].ToString();
                        model.ID = name;
                        dataInfos.Add(model);
                    }
                    this.dataGrid_Demo.ItemsSource = dataInfos;
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 复选框事件
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((System.Windows.Controls.CheckBox)sender).IsChecked = !((System.Windows.Controls.CheckBox)sender).IsChecked;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 全选复选框（列头）事件
        /// </summary>
        private void CheckBox_All_SelectClick(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            ObservableCollection<DataInfo> tmpParts = (ObservableCollection<DataInfo>)this.dataGrid_Demo.ItemsSource;
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    data.Selected = cb.IsChecked.Value;
                }
            }
            this.dataGrid_Demo.ItemsSource = tmpParts;
        }

        /// <summary>
        /// datagrid 事件，改变复选框状态
        /// </summary>
        private void dataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int tempIndex = dataGrid_Demo.SelectedIndex;
            if (dataGrid_Demo.SelectedItems.Count > 1)
            {
                bool isChecked = (dataGrid_Demo.SelectedItems[0] as DataInfo).Selected;
                foreach (var item in dataGrid_Demo.SelectedItems)
                {
                    if (item is DataInfo)
                    {

                        (item as DataInfo).Selected = isChecked;
                    }
                }
            }
            else
            {
                foreach (var item in dataGrid_Demo.SelectedItems)
                {
                    if (item is DataInfo)
                    {
                        (item as DataInfo).Selected = !(item as DataInfo).Selected;
                    }
                }
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DataInfo> tmpParts = (ObservableCollection<DataInfo>)this.dataGrid_Demo.ItemsSource;
            string send = "";
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        send += data.ID.Trim() + ";";
                    }
                }
            }
            senduser.Text = send;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            string user = senduser.Text;
            string[] sArray = user.Split(';');
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                foreach (string i in sArray)
                {
                    if (i != "")
                    {
                        string strYMD = DateTime.Now.ToString();
                        string insertsql = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','" + i + "','" + message.Text + "','" + strYMD + "','1')";
                        try
                        {
                            SqlCommand cmd = new SqlCommand(insertsql);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("有误请检查！！" + ex);
                        }
                    }
                }
                conn.Close();
                senduser.Text = "";
                message.Text = "";
                MessageBox.Show("发送成功！！！");
            }
        }

        private void CXWDXX_Click(object sender, RoutedEventArgs e)
        {
            listView_first.Items.Clear();
            string selectsql = "Select * from Message where touser='" + name + "' and state='1'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string fromuser = reader["fromuser"].ToString();
                    string time = reader["time"].ToString();
                    int state = (int)reader["state"];
                    string text = reader["message"].ToString();
                    listView_first.Items.Add(new message(fromuser, time, text, state));
                }
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            object o = listView_first.SelectedItem;
            if (o == null)
                return;
            message a = o as message;
            listView_first.Items.Remove(o);
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            SqlConnection conn = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand();
            string updatesql = "Update Message set state = '0' where touser='" + name + "'and fromuser= '" + a.Fromuser + "'and time='" + a.Time + "'and message='" + a.Text + "'";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = updatesql;
            cmd.ExecuteReader();
            conn.Close();
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            listView_first.Items.Clear();
            string selectsql = "Select * from Message where touser='" + name + "'";
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string fromuser = reader["fromuser"].ToString();
                    string time = reader["time"].ToString();
                    int state = (int)reader["state"];
                    string text = reader["message"].ToString();
                    listView_first.Items.Add(new message(fromuser, time, text, state));
                }
            }
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            object o = listView_first.SelectedItem;
            if (o == null)
                return;
            message a = o as message;
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            SqlConnection conn = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand();
            string updatesql = "Delete Message where touser='" + name + "'and fromuser= '" + a.Fromuser + "'and time='" + a.Time + "'and message='" + a.Text + "'";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = updatesql;
            cmd.ExecuteReader();
            conn.Close();
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            TJZJ tJZJ = new TJZJ();
            tJZJ.Show();

        }

        #region 物资管理
        //private ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
        private void LoadData_WZ()
        {
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        int num = (int)reader["num"];
                        int remain = (int)reader["remain"];
                        string detail = reader["detail"].ToString();
                        WZ model = new WZ(name, num, remain, detail);
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// 复选框事件
        /// </summary>
        private void CheckBox2_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            ((System.Windows.Controls.CheckBox)sender).IsChecked = !((System.Windows.Controls.CheckBox)sender).IsChecked;
            //}
            //catch
            //{
            //}
        }

        /// <summary>
        /// 全选复选框（列头）事件
        /// </summary>
        private void CheckBox2_All_SelectClick(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    data.Selected = cb.IsChecked.Value;
                }
            }
            this.dataGrid_Demo2.ItemsSource = tmpParts;
        }

        /// <summary>
        /// datagrid 事件，改变复选框状态
        /// </summary>
        private void dataGrid2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int tempIndex = dataGrid_Demo2.SelectedIndex;
            if (dataGrid_Demo2.SelectedItems.Count > 1)
            {
                bool isChecked = (dataGrid_Demo2.SelectedItems[0] as WZ).Selected;
                foreach (var item in dataGrid_Demo2.SelectedItems)
                {
                    if (item is WZ)
                    {

                        (item as WZ).Selected = isChecked;
                    }
                }
            }
            else
            {
                foreach (var item in dataGrid_Demo2.SelectedItems)
                {
                    if (item is WZ)
                    {
                        (item as WZ).Selected = !(item as WZ).Selected;
                    }
                }
            }
        }
        #endregion

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[4].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[5].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[6].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Items.Refresh();
            SQXZWZ.Visibility = Visibility.Visible;
            GHXZWZ.Visibility = Visibility.Collapsed;
            if (power == true)
            {
                XGWZ.Visibility = Visibility.Visible;
                SCWZ.Visibility = Visibility.Visible;
            }

            LoadData_WZ();
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            //string send = "";
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        WZIN wzin = new WZIN(data.Name.Trim(), data.Remain, name);
                        wzin.Show();
                    }
                }
            }
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            SQXZWZ.Visibility = Visibility.Collapsed;
            GHXZWZ.Visibility = Visibility.Collapsed;
            XGWZ.Visibility = Visibility.Collapsed;
            SCWZ.Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Items.Refresh();
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Collapsed;
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ_temp");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzname = (string)reader["name"];
                        int num = (int)reader["num"];
                        string detail = (string)reader["detail"];
                        string time = (string)reader["time"];
                        string touser = (string)reader["touser"];
                        int state = (int)reader["state"];
                        WZ model = new WZ(wzname, num, detail, time, touser, state);
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)//驳回
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            string strYMD = DateTime.Now.ToString();
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {

                        string deletesql = "DELETE FROM WZ_temp WHERE name='" + data.Name + "' and touser='" + data.Touser + "' and detail='" + data.Detail + "' and time='" + data.Time + "'";
                        string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                        using (SqlConnection conn = new SqlConnection(str))
                        {
                            conn.Open();
                            try
                            {
                                if (BHLY1.Text == "请填写驳回的理由：")
                                {
                                    MessageBox.Show("请填写驳回的理由：");
                                    return;
                                }
                                else
                                {
                                    SqlCommand cmd = new SqlCommand(deletesql);
                                    cmd.Connection = conn;
                                    cmd.ExecuteNonQuery();
                                    string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','" + data.Touser + "','我向你发送了物资申请驳回，理由为：" + BHLY1.Text + "','" + strYMD + "','1')";
                                    cmd = new SqlCommand(insertsql2, conn);
                                    cmd.ExecuteNonQuery();
                                    BHLY.Text = "请填写驳回的理由：";
                                }

                            }
                            catch
                            {

                            }
                            conn.Close();
                        }
                        dataGrid_Demo2.Items.Refresh();
                        LoadData_WZ();
                    }
                }
            }
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            string strYMD = DateTime.Now.ToString();
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        string deletesql = "insert into WZ_out select * from WZ_temp WHERE name='" + data.Name + "' and num='" + data.Num + "' and detail='" + data.Detail + "'and time='" + data.Time + "'" + " DELETE FROM WZ_temp WHERE name='" + data.Name + "' and num='" + data.Num + "' and detail='" + data.Detail + "'and time='" + data.Time + "'";
                        string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                        string selectsql = "SELECT * from WZ where name='" + data.Name + "'";
                        using (SqlConnection conn = new SqlConnection(str))
                        {
                            conn.Open();
                            //try
                            //{
                            SqlCommand cmd = new SqlCommand(deletesql);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand(selectsql);
                            cmd.Connection = conn;
                            SqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            int remain = (int)reader["remain"];
                            remain -= data.Num;
                            string updatetsql = "Update WZ set remain ='" + remain + "' where name='" + data.Name + "'";
                            reader.Close();
                            cmd = new SqlCommand(updatetsql);
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                            string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','" + data.Touser + "','我同意了你的物资申请','" + strYMD + "','1')";
                            cmd = new SqlCommand(insertsql2, conn);
                            cmd.ExecuteNonQuery();
                            // }
                            //catch
                            //{

                            // }
                            conn.Close();
                        }
                    }
                    dataGrid_Demo2.Items.Refresh();
                    LoadData_WZ();
                }
            }

        }

        private void Button_Click_19(object sender, RoutedEventArgs e)//查询所有已借
        {
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[4].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[5].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[6].Visibility = Visibility.Visible;
            dataGrid_Demo2.Items.Refresh();
            SQXZWZ.Visibility = Visibility.Collapsed;
            GHXZWZ.Visibility = Visibility.Collapsed;
            XGWZ.Visibility = Visibility.Collapsed;
            SCWZ.Visibility = Visibility.Collapsed;
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ_out where state='1'");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzname = (string)reader["name"];
                        int num = (int)reader["num"];
                        string detail = (string)reader["detail"];
                        string time = (string)reader["time"];
                        string touser = (string)reader["touser"];
                        int state = (int)reader["state"];
                        WZ model = new WZ(wzname, num, detail, time, touser, state);
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)//查询自己已借
        {
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[4].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[5].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[6].Visibility = Visibility.Visible;
            dataGrid_Demo2.Items.Refresh();
            SQXZWZ.Visibility = Visibility.Collapsed;
            GHXZWZ.Visibility = Visibility.Visible;
            XGWZ.Visibility = Visibility.Collapsed;
            SCWZ.Visibility = Visibility.Collapsed;
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ_out where state='1' and touser='" + name + "'");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzname = (string)reader["name"];
                        int num = (int)reader["num"];
                        string detail = (string)reader["detail"];
                        string time = (string)reader["time"];
                        string touser = (string)reader["touser"];
                        int state = (int)reader["state"];
                        WZ model = new WZ(wzname, num, detail, time, touser, state);
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }

        private void Button_Click_21(object sender, RoutedEventArgs e)//归还已选物资
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            string strYMD = DateTime.Now.ToString();
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        string deletesql = "UPDATE WZ_out set state='0' , Gtime='" + strYMD + "' WHERE name='" + data.Name + "' and num='" + data.Num + "' and detail='" + data.Detail + "'and time='" + data.Time + "'";
                        string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                        string selectsql = "SELECT * from WZ where name='" + data.Name + "'";
                        using (SqlConnection conn = new SqlConnection(str))
                        {
                            conn.Open();
                            try
                            {
                                SqlCommand cmd = new SqlCommand(deletesql);
                                cmd.Connection = conn;
                                cmd.ExecuteNonQuery();
                                cmd = new SqlCommand(selectsql);
                                cmd.Connection = conn;
                                SqlDataReader reader = cmd.ExecuteReader();
                                reader.Read();
                                int remain = (int)reader["remain"];
                                remain += data.Num;
                                string updatetsql = "Update WZ set remain ='" + remain + "' where name='" + data.Name + "'";
                                reader.Close();
                                cmd = new SqlCommand(updatetsql);
                                cmd.Connection = conn;
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {

                            }
                            conn.Close();
                        }
                        MessageBox.Show("归还成功！");
                    }
                }
            }
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[4].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[5].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[6].Visibility = Visibility.Visible;
            dataGrid_Demo2.Items.Refresh();
            SQXZWZ.Visibility = Visibility.Collapsed;
            GHXZWZ.Visibility = Visibility.Collapsed;
            XGWZ.Visibility = Visibility.Collapsed;
            SCWZ.Visibility = Visibility.Collapsed;
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ_out");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzname = (string)reader["name"];
                        int num = (int)reader["num"];
                        string detail = (string)reader["detail"];
                        string time = (string)reader["time"];
                        string touser = (string)reader["touser"];
                        int state = (int)reader["state"];
                        string gtime;
                        if (state == 0)
                        {
                            gtime = (string)reader["Gtime"];
                        }
                        else
                        {
                            gtime = "未还";
                        }
                        WZ model = new WZ(wzname, num, detail, time, touser, state);
                        model.GTime = gtime;
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }

        private void Button_Click_23(object sender, RoutedEventArgs e)
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            string strYMD = DateTime.Now.ToString();
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        WZTJ wZTJ = new WZTJ(data.Name);
                        wZTJ.Show();

                    }
                }
            }
        }

        private void Button_Click_24(object sender, RoutedEventArgs e)
        {
            ObservableCollection<WZ> tmpParts = (ObservableCollection<WZ>)this.dataGrid_Demo2.ItemsSource;
            if (tmpParts != null)
            {
                foreach (var data in tmpParts)
                {
                    if (data.Selected)
                    {
                        if (data.Num != data.Remain)
                        {
                            MessageBox.Show(data.Name + "有借出情况不能删除，建议修改物资操作！！");
                        }
                        else
                        {
                            string deletesql = "delete WZ where name='" + data.Name + "'";
                            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
                            using (SqlConnection conn = new SqlConnection(str))
                            {
                                conn.Open();
                                try
                                {
                                    SqlCommand cmd = new SqlCommand(deletesql);
                                    cmd.Connection = conn;
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show(data.Name + "删除成功！！");
                                }
                                catch
                                {

                                }
                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        private void Button_Click_25(object sender, RoutedEventArgs e)//查询自己的记录
        {
            dataGrid_Demo2.Columns[3].Visibility = Visibility.Collapsed;
            dataGrid_Demo2.Columns[4].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[5].Visibility = Visibility.Visible;
            dataGrid_Demo2.Columns[6].Visibility = Visibility.Visible;
            dataGrid_Demo2.Items.Refresh();
            SQXZWZ.Visibility = Visibility.Collapsed;
            GHXZWZ.Visibility = Visibility.Collapsed;
            XGWZ.Visibility = Visibility.Collapsed;
            SCWZ.Visibility = Visibility.Collapsed;
            ObservableCollection<WZ> wzInfos = new ObservableCollection<WZ>();
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from WZ_out where touser='" + name + "'");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzname = (string)reader["name"];
                        int num = (int)reader["num"];
                        string detail = (string)reader["detail"];
                        string time = (string)reader["time"];
                        string touser = (string)reader["touser"];
                        int state = (int)reader["state"];
                        string gtime;
                        if (state == 0)
                        {
                            gtime = (string)reader["Gtime"];
                        }
                        else
                        {
                            gtime = "未还";
                        }
                        WZ model = new WZ(wzname, num, detail, time, touser, state);
                        model.GTime = gtime;
                        wzInfos.Add(model);
                    }
                    this.dataGrid_Demo2.ItemsSource = wzInfos;
                }
                catch
                {

                }
            }
        }

        private void Button_Click_26(object sender, RoutedEventArgs e)
        {
            WZTJ wZTJ = new WZTJ();
            wZTJ.Show();
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object o = listView_first.SelectedItem;
            if (o == null)
                return;
            message a = o as message;
            MessageBox.Show(a.Text);
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            SqlConnection conn = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand();
            string updatesql = "Update Message set state = '0' where touser='" + name + "'and fromuser= '" + a.Fromuser + "'and time='" + a.Time + "'and message='" + a.Text + "'";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = updatesql;
            cmd.ExecuteReader();
            conn.Close();
            listView_first.Items.Remove(o);
        }

        private void Button_Click_27(object sender, RoutedEventArgs e)
        {
            List<string> username = new List<string>();
            string dirpath = test.Text.Trim();
            string[] sArray = Regex.Split(dirpath, "/", RegexOptions.IgnoreCase);
            string strYMD = DateTime.Now.ToString();
            File.WriteAllText(dirpath + "/user.txt", strYMD, Encoding.Default);
            DirectoryInfo d = new DirectoryInfo(dirpath);
            IPAddress ip = IPAddress.Parse("SQL地址");
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse("2020"));
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                string selectsql = "select * from login";
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tempname = (string)reader["name"];
                    username.Add(tempname);
                }
                reader.Close();
                conn.Close();
            }
            try
            {
                ShowMsg(information, "与服务器连接中。。。 。。。");
                sockClient.Connect(endPoint);
                if (d.Exists)
                {
                    FileInfo[] filenames = null;
                    try { filenames = d.GetFiles(); }
                    catch { return; }
                    foreach (FileInfo fil in filenames)
                    {
                        using (FileStream fs = new FileStream(dirpath + "/" + fil.ToString(), FileMode.Open))
                        {
                            string fileName = sArray[1] + "/" + sArray[2] + "/" + sArray[3] + "/" + System.IO.Path.GetFileName(dirpath + "/" + fil.ToString()) + "###";//文件的名字+扩展名，方便后面的保存操作；
                            byte[] arrFile = new byte[1024 * 1024 * 2];
                            int length = fs.Read(arrFile, 0, arrFile.Length);  // 将文件中的数据读到arrFile数组中；  
                            byte[] arrFileName = System.Text.Encoding.Default.GetBytes(fileName);
                            int length2 = arrFileName.Length;
                            byte[] arrFileSend = new byte[length + length2 + 1];

                            arrFileSend[0] = 1; // 用来表示发送的是文件数据；  
                            Buffer.BlockCopy(arrFileName, 0, arrFileSend, 1, length2);
                            Buffer.BlockCopy(arrFile, 0, arrFileSend, length2 + 1, length);
                            sockClient.Send(arrFileSend);// 发送数据到服务端；  
                            System.Threading.Thread.Sleep(1000);
                            using (SqlConnection conn = new SqlConnection(str))
                            {
                                conn.Open();
                                foreach (string dinosaur in username)
                                {
                                    string insertsql2 = "INSERT INTO Message(fromuser,touser,message,time,state) VALUES('" + name + "','" + dinosaur + "','我修改了手册，请到：http://daitianyu.cn/" + sArray[1] + "/" + sArray[2] + "/" + sArray[3] + "/" + System.IO.Path.GetFileName(dirpath + "/" + fil.ToString()) + "下载','" + strYMD + "','1')";
                                    SqlCommand cmd2 = new SqlCommand(insertsql2, conn);
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                return;
            }
            ShowMsg(information, "更新成功！！！");

        }
        private void TB_refister(object sender, RoutedEventArgs e)
        {

            register RE = new register();
            RE.Show();
            //this.Close();
        }

        private void Button_Click_28(object sender, RoutedEventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("名称", typeof(string));
            dt.Columns.Add("数量", typeof(int));
            dt.Columns.Add("详细说明", typeof(string));
            string str = @"Data Source=SQL地址;Initial Catalog=CWXT;User ID=sa;Password=SQL密码";
            
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    DataRow row;
                    SqlCommand cmd = new SqlCommand("select * from WZ");
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                            row = dt.NewRow();
                            row["名称"] = (string)reader["name"];
                            row["数量"] = (int)reader["num"];
                            row["详细说明"] = (string)reader["detail"];
                            dt.Rows.Add(row);
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

                    excelWB.SaveAs("D:\\CW\\WZ.xlsx");  //将其进行保存到指定的路径
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
    }
}
