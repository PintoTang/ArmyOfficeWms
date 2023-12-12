using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.View;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl.Domain;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            DataContext = this;
            BindTaskTypeButtonName();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    GetPieSeriesData();
                    Thread.Sleep(15000);//30秒刷新一下
                }
            });            
        }

        private void BindTaskTypeButtonName()
        {
            try
            {
                //var ucChildrens = TaskTypeGrid.Children;
                //for (int i = ucChildrens.Count - 1; i >= 0; i--)
                //{
                //    if (ucChildrens[i] is Button && (ucChildrens[i] as Button).Tag.ToString() != "0")
                //    {
                //        var ccc = TaskTypeConfig.Instance.TaskButtonList;
                //        (ucChildrens[i] as Button).Content = TaskTypeConfig.Instance.TaskButtonList[int.Parse((ucChildrens[i] as Button).Tag.ToString())-1].Name;
                //    }
                //}

                foreach(var item in TaskTypeConfig.Instance.TaskButtonList.Where(x=>x.Show==true))
                {
                    Button button=new Button();
                    button.Tag = item.Id;
                    button.Content = item.Name;
                    button.FontSize = 20; 
                    button.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    button.Width = 135;button.Height = 55;
                    button.Click += Button_Click;
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = new BitmapImage(new Uri("Images/btnBackground.png", UriKind.Relative));
                    button.Background = imageBrush;
                    Grid.SetRow(button, item.Row);
                    Grid.SetColumn(button, item.Column);
                    TaskTypeGrid.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("配置异常：" + ex.Message);
            }
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

        private void GetPieSeriesData()
        {
            List<string> titles = new List<string>();
            List<double> pieValues = new List<double>();
            foreach (var dict in InvStatusDict)
            {
                titles.Add(dict.Value);
                var invQty = _wmsDataService.GetInvQtyByStatus(dict.Key);
                pieValues.Add(invQty);
            }
            Dispatcher.BeginInvoke
                (new Action
                        (delegate
                                    {
                                        PieSeriesCollection.Clear();
                                        ChartValues<double> chartvalue = new ChartValues<double>();
                                        for (int i = 0; i < titles.Count; i++)
                                        {
                                            chartvalue = new ChartValues<double>();
                                            chartvalue.Add(pieValues[i]);
                                            PieSeries series = new PieSeries();
                                            series.FontSize = 26;
                                            series.DataLabels = true;
                                            series.Title = titles[i]+"   ";
                                            series.Values = chartvalue;
                                            PieSeriesCollection.Add(series);
                                        }
                                    }
                        )
               );
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
                if (selectedSeries.Title.Contains(dict.Value))
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
            //SendTcpCommand();
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
            inventoryList.ViewModel.CurTaskType = int.Parse((sender as Button).Tag.ToString());
            inventoryList.ViewModel.SearchCommand.Execute(null);
        }

        private void SendTcpCommand()
        {
            try
            {
                TcpCom tcp = new TcpCom();
                tcp.RemoteIp = IPAddress.Parse("192.168.1.33");
                tcp.RemoteIpPort = 20108;
                if (tcp.Connected==false)
                {
                    tcp.Connect();
                }
                if (tcp.Connected==false)
                {
                    SnackbarQueue.MessageQueue.Enqueue("TCP连接失败！");
                }
                //EF AA 06 AA 16 03 02 00 05 CA EF 55
                //EF AA 02 AA 16 03 02 00 01 C6 EF 55
                byte[] buffer = new byte[12];
                buffer[0] = 0xEF;
                buffer[1] = 0xAA;
                buffer[2] = 0x06;
                buffer[3] = 0xAA;
                buffer[4] = 0x16;
                buffer[5] = 0x03;
                buffer[6] = 0x02;
                buffer[7] = 0x00;
                buffer[8] = 0x05;
                buffer[9] = 0xCA;
                buffer[10] = 0xEF;
                buffer[11] = 0x55;
                tcp.Send(buffer);
                Thread.Sleep(250);
                tcp.Send(buffer);
            }
            catch { }
        }

        private void btnInOrder_Click(object sender, RoutedEventArgs e)
        {
            UcInOrderList inOrderList = new UcInOrderList();
            UserContentControl.Children.Add(inOrderList);            
        }

        private void btnInvenroty_Click(object sender, RoutedEventArgs e)
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
            inventoryList.ViewModel.SearchCommand.Execute(null);
        }

        private void btnOutOrder_Click(object sender, RoutedEventArgs e)
        {
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                if (ucChildrens[i] is UcOutOrderList)
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            UcOutOrderList outOrderList = new UcOutOrderList();
            UserContentControl.Children.Add(outOrderList);
            //inventoryList.ViewModel.SearchCommand.Execute(null);
        }

        private void btnSystemSetup_Click(object sender, RoutedEventArgs e)
        {
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                if (ucChildrens[i] is SystemConfigView)
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            SystemConfigView systemConfig = new SystemConfigView();
            UserContentControl.Children.Add(systemConfig);
            //inventoryList.ViewModel.SearchCommand.Execute(null);
        }



    }
}
