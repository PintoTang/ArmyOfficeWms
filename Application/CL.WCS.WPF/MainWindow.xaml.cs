using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.Framework.Log.Helper;
using CLDC.CLWCS.Service.MenuService;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.StartService;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Domain;
using MenuItem = CLDC.Infrastructrue.UserCtrl.Model.MenuItem;

namespace CL.WCS.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 管理所有的UserControl对象
        /// </summary>
        private readonly Dictionary<string, UserControl> _dicUserCtrl = new Dictionary<string, UserControl>();

        /// <summary>
        /// 图标
        /// </summary>
        readonly System.Windows.Forms.NotifyIcon _smallNotify = new System.Windows.Forms.NotifyIcon();

        private readonly ObservableCollection<MenuItem> _mainMenuItem = new ObservableCollection<MenuItem>();


        public MainWindow()
        {
            try
            {
                InitializeComponent();

                SetWindowStartUpLocation();//系统窗体打开时的位置和大小
                LoadSystemIcon();//加载系统ICO 图标
                RegisterEvent();
                MessageViewModel messageViewModel = new MessageViewModel();
                this.Snackbar.DataContext = messageViewModel;

                DelegateContainer.MaxMonitorScreen += MaxScreen;
                DelegateContainer.MinMonitoScreen += MinScreen;


            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("未处理异常", ex);
            }

        }


        private void AddUseCtrl(string userCtrlId, UserControl userCtrl)
        {
            if (!_dicUserCtrl.ContainsKey(userCtrlId))
            {
                _dicUserCtrl.Add(userCtrlId, userCtrl);
            }
        }

        private void InitilizeMenu()
        {
            if (WcsMenuManage.Instance.IsEmpty)
            {
                return;
            }
            foreach (WcsMenuAbstract menuAbstract in WcsMenuManage.Instance.ManagedDataPool.DataPool)
            {
                WcsMenuAbstract deviceMonitor = (WcsMenuAbstract)Assembly.Load(menuAbstract.NameSpace)
                                    .CreateInstance(menuAbstract.NameSpace + "." + menuAbstract.ClassName);
                if (deviceMonitor == null)
                {
                    LogHelper.WriteLog("加载菜单", String.Format("根据命名空间：{0} 类名：{1} 加载菜单为空", menuAbstract.NameSpace, menuAbstract.ClassName), EnumLogLevel.Error);
                    continue;
                }
                OperateResult initilizeResult = deviceMonitor.Initilize();

                if (!initilizeResult.IsSuccess)
                {
                    LogHelper.WriteLog("加载菜单", String.Format("菜单：{0} 初始化失败：{1}", menuAbstract.Name, initilizeResult.Message), EnumLogLevel.Error);
                    continue;
                }
                deviceMonitor.ClassName = menuAbstract.ClassName;
                deviceMonitor.Name = menuAbstract.Name;
                deviceMonitor.IconKind = menuAbstract.IconKind;
                deviceMonitor.Id = menuAbstract.Id;
                deviceMonitor.IsShowUi = menuAbstract.IsShowUi;
                UserControl menuView = deviceMonitor.GetDetailView();
                IMainUseCtrl IMainView = menuView as IMainUseCtrl;
                if (IMainView == null)
                {
                    LogHelper.WriteLog("加载菜单", String.Format("菜单：{0} 不实现IMainUseCtrl", menuAbstract.Name), EnumLogLevel.Error);
                    continue;
                }
                UserContentControl.Children.Add(menuView);
                AddUseCtrl(deviceMonitor.Name, menuView);
                _mainMenuItem.Add(new MenuItem(deviceMonitor.Name, deviceMonitor.IconKind));
            }
            var first = WcsMenuManage.Instance.ManagedDataPool.DataPool.FirstOrDefault();
            SetShowRenderControl(first.Name);
        }






        /// <summary>
        /// 主窗体初始化对象加载
        /// </summary>
        private void MainRenderInitialization()
        {

            //将所有的子集控件添加进去

            /*******************************************************************************
             * 
             *    例如此处展示了文件控件是如何添加进去的 
             *    1.先进行实例化，赋值初始参数
             *    2.添加进项目
             *    3.显示
             *
             *******************************************************************************/

            try
            {

                InitilizeMenu();

                LbCopyRight.Content = string.Format("本软件著作权归 {0} 所有", SystemConfig.Instance.CopyRight);
                LbSystemName.Content = SystemConfig.Instance.SysName;
                LbVersion.Content = string.Format("系统版本：{0}", SystemConfig.Instance.Version);


                ListBoxMenu.ItemsSource = _mainMenuItem;
                ListBoxMenuMin.ItemsSource = _mainMenuItem;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        private string _curShowCtrlId = string.Empty;

        private void MaxScreen()
        {
            TitleZone.Visibility = Visibility.Collapsed;
            ToolZone.Visibility = Visibility.Collapsed;
            BottomZone.Visibility = Visibility.Collapsed;
        }

        private void MinScreen()
        {
            TitleZone.Visibility = Visibility.Visible;
            ToolZone.Visibility = Visibility.Visible;
            BottomZone.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 设置窗体启动居中方式和大小
        /// </summary>
        private void SetWindowStartUpLocation()
        {
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
        }
        /// <summary>
        /// 加载系统ICO图标
        /// </summary>
        private void LoadSystemIcon()
        {
            _smallNotify.BalloonTipText = "智能仓库管理系统V2.0";
            _smallNotify.Text = "智能仓库管理系统V2.0";
            _smallNotify.Visible = true;
            _smallNotify.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _smallNotify.MouseDoubleClick += smallNotify_MouseDoubleClick;
            _smallNotify.ShowBalloonTip(1000);
        }
        void smallNotify_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                SetWindowStartUpLocation();

            }
            else
            {
                this.WindowState = System.Windows.WindowState.Minimized;

            }
        }


        private void ChangeShow(string useCtrlId)
        {
            foreach (var item in _dicUserCtrl)
            {
                if (!(item.Value is IMainUseCtrl))
                {
                    continue;
                }
                IMainUseCtrl tempCtrl = (IMainUseCtrl)item.Value;
                if (String.CompareOrdinal(item.Key, useCtrlId) == 0)
                {
                    if (item.Value.Visibility == System.Windows.Visibility.Visible)
                    {
                        continue;
                    }
                    tempCtrl.Show();
                }
                else
                {
                    if (item.Value.Visibility == System.Windows.Visibility.Hidden)
                    {
                        continue;
                    }
                    tempCtrl.Hide();
                }
            }
        }

        private static readonly object LockSwitchView = new object();

        private void SetShowRenderControl(string useCtrlId)
        {
            lock (LockSwitchView)
            {
                try
                {
                    if (_curShowCtrlId.Equals(useCtrlId))
                    {
                        return;
                    }
                    DoubleAnimation dOpacity = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                    DoubleAnimation dY = new DoubleAnimation(0, UserContentControl.ActualWidth, TimeSpan.FromMilliseconds(300));
                    TranslateTransform tt = new TranslateTransform();
                    UserContentControl.RenderTransform = tt;
                    dOpacity.Completed += delegate
                    {
                        try
                        {
                            DoubleAnimation dOpacity2 = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                            DoubleAnimation dY2 = new DoubleAnimation(-UserContentControl.ActualWidth, 0, TimeSpan.FromMilliseconds(300));
                            TranslateTransform tt2 = new TranslateTransform();
                            UserContentControl.RenderTransform = tt2;
                            tt2.BeginAnimation(TranslateTransform.XProperty, dY2);
                            UserContentControl.BeginAnimation(OpacityProperty, dOpacity2);
                            ChangeShow(useCtrlId);
                            _curShowCtrlId = useCtrlId;
                        }
                        catch (Exception ex)
                        {
                            MessageBoxEx.Show(ex.Message, "错误");
                        }
                    };
                    UserContentControl.BeginAnimation(OpacityProperty, dOpacity);
                    tt.BeginAnimation(TranslateTransform.XProperty, dY);
                    MenuToggleButton.IsChecked = false;
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
        }


        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvent()
        {
            TitleZone.OnMinClick += OnMinClick;
            TitleZone.OnCloseApp += OnCloseApp;
            TitleZone.OnLogout += OnLogoutApp;
            TitleZone.DragMove += delegate { DragMove(); }; //拖拽
        }

        void OnCloseApp()
        {
            MessageBoxResult msg = MessageBoxEx.Show("是否退出系统?", "提示", MessageBoxButton.OKCancel);
            if (msg == MessageBoxResult.OK)
            {

                App.SystemCurMode = SystemMode.Exiting;
                Close();
            }
        }

        void OnLogoutApp()
        {
            MessageBoxResult msg = MessageBoxEx.Show("是否注销系统?", "提示", MessageBoxButton.OKCancel);
            if (msg == MessageBoxResult.OK)
            {
                _smallNotify.Dispose();
                App.SystemCurMode = SystemMode.LogOut;
                Close();
            }
        }

        private void OnMinClick()
        {
            this.WindowState = System.Windows.WindowState.Minimized;

        }

        /// <summary>
        /// 窗体 消息显示
        /// </summary>
        /// <param name="msg"></param>
        public void ShowMessage(string msg)
        {
            this.Dispatcher.Invoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg });
        }

        const string LogName = "WPF界面加载";

        delegate void MessageBoxShow(string msg);
        void MessageBoxShow_F(string msg)
        {
            LogHelper.WriteLog(LogName, msg, EnumLogLevel.Error);
            MessageBoxResult result = System.Windows.MessageBox.Show(msg, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
            if (result.Equals(MessageBoxResult.OK))
            {
                Environment.Exit(0);
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _smallNotify.Dispose();
        }

        private bool _isMessageShow = false;
        private void BtnMessageShow_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isMessageShow)
            {
                HideMessageTool();
                _isMessageShow = false;
            }
            else
            {
                ShowMessageTool();
                _isMessageShow = true;
            }
        }

        private void ShowMessageTool()
        {
            TranslateTransform transform = new TranslateTransform();
            GridMessage.RenderTransform = transform;
            DoubleAnimation showAnimation = new DoubleAnimation(0, -GridMessage.ActualWidth, TimeSpan.FromMilliseconds(250), FillBehavior.HoldEnd);
            transform.BeginAnimation(TranslateTransform.XProperty, showAnimation);
        }

        private void HideMessageTool()
        {
            TranslateTransform transform = new TranslateTransform();
            GridMessage.RenderTransform = transform;
            DoubleAnimation hideAnimation = new DoubleAnimation(-GridMessage.ActualWidth, 0, TimeSpan.FromMilliseconds(250), FillBehavior.HoldEnd);
            transform.BeginAnimation(TranslateTransform.XProperty, hideAnimation);
        }

        private void GridMessage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isMessageShow)
            {
                return;
            }
            HideMessageTool();
            _isMessageShow = false;
        }

        private void GridMessage_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //if (_isMessageShow)
            //{
            //    return;
            //}
            //ShowMessageTool();
            //_isMessageShow = true;
        }

        private void ListBoxMenuMin_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb == null)
            {
                return;
            }
            MenuItem selectedItem = (MenuItem)lb.SelectedValue;
            if (selectedItem == null)
            {
                return;
            }
            SetShowRenderControl(selectedItem.Title);
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(MainRenderInitialization, DispatcherPriority.Loaded);
        }
    }
}
