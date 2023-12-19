using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView.Tools;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcInventoryList.xaml 的交互逻辑
    /// </summary>
    public partial class UcInventoryList : UserControl
    {
        public InventoryListViewModel ViewModel { get; set; }
        public UcInventoryList()
        {
            InitializeComponent();
            ViewModel = new InventoryListViewModel();
            DataContext = ViewModel;
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void BtnExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hide();
        }

        private void InventoryGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbArea.SelectedIndex == -1 || cbTeam.SelectedIndex == -1)
                {
                    MessageBoxEx.Show("未选择任务分类或者分队，不能定位!");
                    return;
                }

                TcpCom tcp = new TcpCom();
                tcp.RemoteIp = IPAddress.Parse(SystemConfig.Instance.RemoteIp);
                tcp.RemoteIpPort = 20108;
                if (tcp.Connected == false)
                {
                    tcp.Connect();
                }
                if (tcp.Connected == false)
                {
                    SnackbarQueue.MessageQueue.Enqueue("TCP连接失败！");
                }

                var command = SoundLightConfig.Instance.CommandList.FirstOrDefault(x => x.Area == (string)cbArea.SelectedValue && x.Team == (string)cbTeam.SelectedValue);
                if (command == null)
                {
                    MessageBoxEx.Show("未配置此区域的声光报警指令，请重新配置!");
                    return;
                }
                else
                {
                    string[] strCode = command.Code.Split(' ');
                    byte[] buffer = new byte[strCode.Length];
                    buffer = ToBytesFromHexString(command.Code);
                    tcp.Send(buffer);
                    Thread.Sleep(250);
                    tcp.Send(buffer);
                }
            }
            catch { }
        }



        /// <summary>
        /// 16进制格式字符串转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] ToBytesFromHexString(string hexString)
        {
            //以 ' ' 分割字符串，并去掉空字符
            string[] chars = hexString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] returnBytes = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(chars[i], 16);
            }
            return returnBytes;
        }

        private async void btnQuickTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbArea.SelectedIndex == -1 || cbTeam.SelectedIndex == -1)
                {
                    MessageBoxEx.Show("请选择任务分类与任务分队!");
                    return;
                }

                TcpCom tcp = new TcpCom();
                tcp.RemoteIp = IPAddress.Parse(SystemConfig.Instance.RemoteIp);
                tcp.RemoteIpPort = 20108;
                if (tcp.Connected == false)
                {
                    tcp.Connect();
                }
                if (tcp.Connected == false)
                {
                    SnackbarQueue.MessageQueue.Enqueue("TCP连接失败！");
                }

                var command = SoundLightConfig.Instance.CommandList.FirstOrDefault(x => x.Area == (string)cbArea.SelectedValue && x.Team == (string)cbTeam.SelectedValue);
                if (command == null)
                {
                    MessageBoxEx.Show("未配置此区域的声光报警指令，请重新配置!");
                    return;
                }
                else
                {
                    ISpeech speech = new SpeechBussiness();
                    speech.SpeakAsync(command.SoundContent);


                    string[] strCode = command.Code.Split(' ');
                    byte[] buffer = new byte[strCode.Length];
                    buffer = ToBytesFromHexString(command.Code);
                    tcp.Send(buffer);
                    Thread.Sleep(250);
                    tcp.Send(buffer);


                    CreateOutOrderView createOutOrder = new CreateOutOrderView("1");
                    createOutOrder.progressLoop.Visibility = Visibility.Visible;
                    CreateOrderViewModel.SingleInstance.ScanCommand.Execute(null);
                    CreateOrderViewModel.SingleInstance.CurArea = (string)cbArea.SelectedValue;
                    CreateOrderViewModel.SingleInstance.CurTeam = (string)cbTeam.SelectedValue;
                    await MaterialDesignThemes.Wpf.DialogHost.Show(createOutOrder, "DialogHostWait");                    
                }
            }
            catch { }
        }


    }
}
