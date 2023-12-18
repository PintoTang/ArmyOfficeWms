using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NPOI.SS.Formula.PTG;
using System;
using System.Net;
using CLDC.CLWS.CLWCS.Service.WmsView.Tools;

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
                    for (int i = 0; i < 2; i++)
                    {
                        SpeakHelper.SpeekContent(command.SoundContent);
                        Thread.Sleep(2000);
                    }


                    string[] strCode = command.Code.Split(' ');
                    byte[] buffer = new byte[strCode.Length];
                    buffer = ToBytesFromHexString(command.Code);
                    tcp.Send(buffer);
                    Thread.Sleep(250);
                    tcp.Send(buffer);


                    CreateOutOrderView createOutOrder = new CreateOutOrderView("1");
                    await MaterialDesignThemes.Wpf.DialogHost.Show(createOutOrder, "DialogHostWait");
                }
            }
            catch { }
        }


    }
}
