using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl.Domain;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

namespace CLDC.CLWCS.Service.MenuService.View
{
    /// <summary>
    /// MainContentView.xaml 的交互逻辑
    /// </summary>
    public partial class MainContentView : UserControl, IMainUseCtrl
    {
        public MainContentView()
        {
            InitializeComponent();
            this.DataContext = new SystemConfigViewMode(SystemConfig.Instance.CurSystemConfig);
            UserControl menuView = new UcDefaultView();
            UserContentControl.Children.Add(menuView);
            PointLabel = chartPoint => string.Format("{0}({1:p})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        private void pipChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;
            //clear selected slice
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;
                var selectedSeries = (PieSeries)chartPoint.SeriesView;
                selectedSeries.PushOut = 8;
            }
        }

        private string _useCtrlId = "系统主界面";

        public string UseCtrlId
        {
            get { return _useCtrlId; }
            set { _useCtrlId = value; }

        }
        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
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

        private void btnInOrder_Click(object sender, RoutedEventArgs e)
        {
            UcInOrderManage inOrderManage = new UcInOrderManage();
            UserContentControl.Children.Add(inOrderManage);
        }


    }
}
