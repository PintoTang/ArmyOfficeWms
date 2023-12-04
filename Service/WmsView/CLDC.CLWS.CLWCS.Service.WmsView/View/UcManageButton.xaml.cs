using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.Authorize.View;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcManageButton.xaml 的交互逻辑
    /// </summary>
    public partial class UcManageButton : UserControl
    {
        public UcManageButton()
        {
            InitializeComponent();
            DataContext = new ManageButtonViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpCom tcp = new TcpCom();
                tcp.RemoteIp = IPAddress.Parse("192.168.1.8");
                tcp.RemoteIpPort = 20108;

                if (!tcp.Connected)
                {
                    tcp.Connect();
                }

                byte[] buffer = new byte[9];
                buffer[0] = 0xEF;
                buffer[1] = 0xAA;
                buffer[2] = 0x01;
                buffer[3] = 0xAA;
                buffer[4] = 0x02;
                buffer[5] = 0x00;
                buffer[6] = 0xAC;
                buffer[7] = 0xEF;
                buffer[8] = 0x55;
                tcp.Send(buffer);
            }
            catch (Exception ex)
            {

            }
        }


    }
}
