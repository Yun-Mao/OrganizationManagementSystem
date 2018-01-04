using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace TCP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Thread threadWatch = null;//负责监听客户端链接请求的线程；
        Socket socketWatch = null;

        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();

        void ShowMsg(RichTextBox txt, string str)
        {
            txt.Dispatcher.Invoke(
            DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                txt.AppendText(str);
            });
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
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
            ShowMsg(richTextBoxMain, "服务器启动监听成功!");
        }

        void WatchConnecting()
        {
            try
            {
                while (true)
                {
                    Socket sokConnection = socketWatch.Accept();
                    //  ListBoxSend.Items.Add(sokConnection.RemoteEndPoint.ToString());
                    ListBoxSend.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    ListBoxSend.Items.Add(sokConnection.RemoteEndPoint.ToString());
                });
                    dict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                    ShowMsg(richTextBoxMain, "客户端链接成功！");
                    Thread thr = new Thread(RecMsg);
                    thr.IsBackground = true;
                    thr.Start(sokConnection);
                    dictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show("异常：" + se.Message);
                return;
            }
        }

        void RecMsg(object sokConnectionparn)
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
                    ShowMsg(richTextBoxMain, "异常：" + se.Message);
                    dict.Remove(sokClient.RemoteEndPoint.ToString());
                    dictThread.Remove(sokClient.RemoteEndPoint.ToString()); ListBoxSend.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
            (ThreadStart)delegate
            {
                ListBoxSend.Items.Remove(sokClient.RemoteEndPoint.ToString());
            });
                    break;
                }
                if (arrMsgRec[0] == 0)
                {
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);
                    ShowMsg(richTextBoxMain, strMsg);
                }
                if (arrMsgRec[0] == 1) // 表示接收到的是文件数据；  
                {

                    try
                    {
                        string strMsg = System.Text.Encoding.Default.GetString(arrMsgRec, 1, length - 1);
                        string[] sArray = Regex.Split(strMsg, "###", RegexOptions.IgnoreCase);
                        byte[] arrFileName = System.Text.Encoding.Default.GetBytes(sArray[0]+"###");
                        string fileSavePath = "C:/inetpub/wwwroot/" + sArray[0];// 获得文件保存的路径；  
                                                                // 创建文件流，然后根据路径创建文件；  
                        //MessageBox.Show(fileSavePath);
                        using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                        {
                            fs.Write(arrMsgRec, 1+ arrFileName.Length, length - 1 - arrFileName.Length);
                            ShowMsg(richTextBoxMain, "文件保存成功：" + fileSavePath);
                        }
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }
                }
            }
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            string strMsg = "服务器" + "\r\n" + "  -->" + textBoxSend.Text.Trim() + "\r\n";
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
            arrSendMsg[0] = 0;
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            string strKey = "";
            foreach (CheckBox item in ListBoxSend.Items)
            {
                if (item.IsChecked == true)
                {
                    //   strKey = item.Content.ToString();
                }
            }
            if (string.IsNullOrEmpty(strKey))
            {
                MessageBox.Show("请选择你要发送的好友：");
            }
            else
            {
                dict[strKey].Send(arrSendMsg);
                ShowMsg(richTextBoxMain, strMsg);
                textBoxSend.Clear();
            }
        }

        private void buttonSendAll_Click(object sender, RoutedEventArgs e)
        {
            string strMsg = "服务器" + "\r\n" + "   -->" + textBoxSend.Text.Trim() + "\r\t";
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
            foreach (Socket s in dict.Values)
            {
                s.Send(arrMsg);
            }
            ShowMsg(richTextBoxMain, strMsg);
            textBoxSend.Clear();
            ShowMsg(richTextBoxMain, "群发完毕~~~");
        }


    }
}
