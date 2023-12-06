using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl.Domain;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;

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
            GetPieSeriesData();
            PointLabel = chartPoint => string.Format("{0}({1:p})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
        }

        void GetPieSeriesData()
        {
            ///饼状图数据绑定
        }


        public Func<ChartPoint, string> PointLabel { get; set; }

        public List<PieSeries> PieSeriesList { get; set; }
        public ObservableCollection<List<PieSeries>> PieSeriesCollection { get; set; }

        private void pipChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            UcInventoryList inventoryList = new UcInventoryList();
            //加载过的子控件就不要重复加载
            UserContentControl.Children.Add(inventoryList);
            //此处需要加系列的判断
            inventoryList.ViewModel.CurInvStatus = (InvStatusEnum)1;
            inventoryList.ViewModel.SearchCommand.Execute(null);
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
