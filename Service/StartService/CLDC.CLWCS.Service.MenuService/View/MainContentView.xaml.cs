using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.View;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl.Domain;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private AuthorizeService _authorizeService;
        public MainContentView()
        {
            InitializeComponent();
            this.DataContext = new SystemConfigViewMode(SystemConfig.Instance.CurSystemConfig);
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            _authorizeService = DependencyHelper.GetService<AuthorizeService>();
            UserControl menuView = new UcDefaultView();
            UserContentControl.Children.Add(menuView);
            DataContext = this;
            CreateTaskTypeButtons();
            GetPieSeriesData();
            InitAdmin();
        }

        /// <summary>
        /// 根据配置文件动态生产任务类型按钮
        /// </summary>
        private void CreateTaskTypeButtons()
        {
            try
            {
                var areaLst = _wmsDataService.GetAreaList(string.Empty);
                if (areaLst.Count > 0)
                {
                    foreach (var item in areaLst)
                    {
                        Button button = new Button();
                        button.Tag = item.AreaCode;
                        button.Content = item.AreaName;
                        button.FontSize = 20;
                        button.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        button.Width = 135; button.Height = 55;
                        button.Click += Button_Click;
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri("Images/btnBackground.png", UriKind.Relative));
                        button.Background = imageBrush;
                        Grid.SetRow(button, (int)item.ROW);
                        Grid.SetColumn(button, (int)item.COLUMN);
                        TaskTypeGrid.Children.Add(button);
                    }
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("配置异常：" + ex.Message);
            }
        }

        private void InitAdmin()
        {
            //查询管理员数据
            int curRoleLevel = (int)CookieService.CurSession.RoleLevel;
            Expression<Func<AccountMode, bool>> whereLambda = t => (int)t.RoleLevel == curRoleLevel;

            OperateResult<List<AccountMode>> accountListResult = _authorizeService.GetAccountList(whereLambda);
            if (!accountListResult.IsSuccess)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                return;
            }
            if (accountListResult.Content != null && accountListResult.Content.Count > 0)
            {
                for (int i = 0; i < accountListResult.Content.Count; i++)
                {
                    var Name = accountListResult.Content[i]?.AccCode;
                    if (i == 0)
                    {
                        imgAdmin1.Source = CreateBitmapImage("/Images/" + Name + ".jpg");
                        lbAdmin1.Content = Name;
                    }
                    else if (i == 1)
                    {
                        imgAdmin2.Source = CreateBitmapImage("/Images/" + Name + ".jpg");
                        lbAdmin2.Content = Name;
                    }
                }
            }
        }

        private BitmapImage CreateBitmapImage(string imgUrl)
        {
            var CurPath = System.IO.Directory.GetCurrentDirectory();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(CurPath+imgUrl);
            bmp.EndInit();
            return bmp;
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

        /// <summary>
        /// 定时获取饼图数据
        /// </summary>
        private void GetPieSeriesData()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    List<string> titles = new List<string>();
                    List<double> pieValues = new List<double>();
                    foreach (var dict in InvStatusDict)
                    {
                        titles.Add(dict.Value);
                        var invQty = _wmsDataService.GetInvQtyByStatus(dict.Key);
                        pieValues.Add(invQty);
                    }
                    Dispatcher.BeginInvoke(new Action
                        (delegate
                            {
                                PieSeriesCollection.Clear();
                                ChartValues<double> chartvalue = new ChartValues<double>();
                                for (int i = 0; i < titles.Count; i++)
                                {
                                    chartvalue = new ChartValues<double>();
                                    chartvalue.Add(pieValues[i]);
                                    PieSeries series = new PieSeries();
                                    series.FontSize = 20;
                                    series.DataLabels = true;
                                    series.Title = titles[i] + "   ";
                                    series.Values = chartvalue;
                                    PieSeriesCollection.Add(series);
                                }
                            }
                        ));
                    Thread.Sleep(int.Parse(SystemConfig.Instance.Interval) * 1000);
                }
            });
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
                        if (ucChildrens[i] is UcDefaultView)
                        {
                            continue;
                        }
                        else
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
                if (ucChildrens[i] is UcDefaultView)
                {
                    continue;
                }
                else
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            UcInventoryList inventoryList = new UcInventoryList();
            UserContentControl.Children.Add(inventoryList);
            inventoryList.ViewModel.CurArea = (sender as Button).Tag.ToString();
            inventoryList.ViewModel.SearchCommand.Execute(null);
        }

        private void btnInOrder_Click(object sender, RoutedEventArgs e)
        {           
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                if (ucChildrens[i] is UcDefaultView)
                {
                    continue;
                }
                else
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            UcInOrderList inOrderList = new UcInOrderList();
            UserContentControl.Children.Add(inOrderList);
            inOrderList.ViewModel.SearchCommand.Execute(((int)InOrOutEnum.入库).ToString());
        }

        private void btnInvenroty_Click(object sender, RoutedEventArgs e)
        {
            //加载过的子控件就不要重复加载
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                if (ucChildrens[i] is UcDefaultView)
                {
                    continue;
                }
                else
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
                if (ucChildrens[i] is UcDefaultView)
                {
                    continue;
                }
                else
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            UcOutOrderList outOrderList = new UcOutOrderList();
            UserContentControl.Children.Add(outOrderList);
            outOrderList.ViewModel.SearchCommand.Execute(((int)InOrOutEnum.出库).ToString());
        }

        private void btnSystemSetup_Click(object sender, RoutedEventArgs e)
        {
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                if (ucChildrens[i] is UcDefaultView)
                {
                    continue;
                }
                else
                {
                    ucChildrens.Remove(ucChildrens[i]);
                }
            }
            UcSetupView ucSetup = new UcSetupView();
            UserContentControl.Children.Add(ucSetup);
        }


    }
}
