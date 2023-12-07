using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.Infrastructrue.UserCtrl.Domain;
using Infrastructrue.Ioc.DependencyFactory;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private WmsDataService _wmsDataService;
        public MainContentView()
        {
            InitializeComponent();
            this.DataContext = new SystemConfigViewMode(SystemConfig.Instance.CurSystemConfig);

            _wmsDataService = DependencyHelper.GetService<WmsDataService>();

            UserControl menuView = new UcDefaultView();
            UserContentControl.Children.Add(menuView);

            PointLabel = chartPoint => string.Format("{0}({1:p})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
            GetPieSeriesData();
        }

        SeriesCollection pieSeriesCollection = new SeriesCollection();
        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection
        {
            get
            {
                return pieSeriesCollection;
            }

            set
            {
                pieSeriesCollection = value;
            }
        }

        private readonly Dictionary<InvStatusEnum, string> _invStatusDict = new Dictionary<InvStatusEnum, string>();
        public Dictionary<InvStatusEnum, string> InvStatusDict
        {
            get
            {
                if (_invStatusDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(InvStatusEnum)))
                    {
                        InvStatusEnum em = (InvStatusEnum)value;
                        _invStatusDict.Add(em, em.GetDescription());
                    }
                }
                return _invStatusDict;

            }
        }

        void GetPieSeriesData()
        {
            List<string> titles = new List<string>();
            List<double> pieValues = new List<double> ();
            foreach (var dict in InvStatusDict)
            {
                titles.Add(dict.Value);
                var invQty = _wmsDataService.GetInvQtyByStatus(dict.Key);
                pieValues.Add(invQty);
            }
            ChartValues<double> chartvalue = new ChartValues<double>();
            for (int i = 0; i < titles.Count; i++)
            {
                chartvalue = new ChartValues<double>();
                chartvalue.Add(pieValues[i]);
                PieSeries series = new PieSeries();
                series.DataLabels = true;
                series.Title = titles[i];
                series.Values = chartvalue;
                PieSeriesCollection.Add(series);
            }
        }


        public Func<ChartPoint, string> PointLabel { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartPoint)
        {
            var chart = (PieChart)chartPoint.ChartView;
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 2;
            }
            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            selectedSeries.PushOut = 10;
            foreach (var dict in InvStatusDict)
            {
                if (selectedSeries.Title == dict.Value)
                {
                    //加载过的子控件就不要重复加载
                    var ucChildrens = UserContentControl.Children;
                    for (int i = ucChildrens.Count - 1; i >= 0; i--)
                    {
                        if (ucChildrens[i] is UcInventoryList)
                        {
                            ucChildrens.Remove(ucChildrens[i]);
                        }
                    }
                    UcInventoryList inventoryList = new UcInventoryList();
                    UserContentControl.Children.Add(inventoryList);
                    inventoryList.ViewModel.CurInvStatus = (InvStatusEnum)dict.Key;
                    inventoryList.ViewModel.SearchCommand.Execute(null);
                }
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
            //try
            //{
            //    TcpCom tcp = new TcpCom();
            //    tcp.RemoteIp = IPAddress.Parse("192.168.1.8");
            //    tcp.RemoteIpPort = 20108;

            //    if (!tcp.Connected)
            //    {
            //        tcp.Connect();
            //    }

            //    byte[] buffer = new byte[9];
            //    buffer[0] = 0xEF;
            //    buffer[1] = 0xAA;
            //    buffer[2] = 0x01;
            //    buffer[3] = 0xAA;
            //    buffer[4] = 0x02;
            //    buffer[5] = 0x00;
            //    buffer[6] = 0xAC;
            //    buffer[7] = 0xEF;
            //    buffer[8] = 0x55;
            //    tcp.Send(buffer);
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void btnInOrder_Click(object sender, RoutedEventArgs e)
        {
            UcInOrderManage inOrderManage = new UcInOrderManage();
            UserContentControl.Children.Add(inOrderManage);
        }


    }
}
