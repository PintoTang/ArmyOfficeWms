using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.WareHouse.Manage.View;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Domain;
using MaterialDesignThemes.Wpf;
using MenuItem = CLDC.Infrastructrue.UserCtrl.Model.MenuItem;

namespace CLDC.CLWCS.Service.MenuService.View
{
    /// <summary>
    /// UcTaskManage.xaml 的交互逻辑
    /// </summary>
    public partial class TaskManageView : UserControl,IMainUseCtrl
    {

        public TaskManageView()
        {
            InitializeComponent();
          
        }

        private readonly ObservableCollection<MenuItem> _mainMenuItem = new ObservableCollection<MenuItem>();

        /// <summary>
        /// 管理所有的UserControl对象
        /// </summary>
        private readonly Dictionary<string, UserControl> _dicUserCtrl = new Dictionary<string, UserControl>();


        private void InitAllView()
        {
            HotOrderView hotOrderView = new HotOrderView();
            DockPanelMain.Children.Add(hotOrderView);
            AddUseCtrl(hotOrderView.UseCtrlId, hotOrderView);
            _mainMenuItem.Add(new MenuItem(hotOrderView.UseCtrlId, PackIconKind.Quicktime));

            HotTransportManageView hotTransportView=new HotTransportManageView();
            DockPanelMain.Children.Add(hotTransportView);
            AddUseCtrl(hotTransportView.UseCtrlId,hotTransportView);
            _mainMenuItem.Add(new MenuItem(hotTransportView.UseCtrlId, PackIconKind.Timer));
            

            HistoryOrderManageView historyOrderView = new HistoryOrderManageView();
            DockPanelMain.Children.Add(historyOrderView);
            AddUseCtrl(historyOrderView.UseCtrlId, historyOrderView);
            _mainMenuItem.Add(new MenuItem(historyOrderView.UseCtrlId, PackIconKind.History));


            HistoryTransportManageView historyTransportView = new HistoryTransportManageView();
            DockPanelMain.Children.Add(historyTransportView);
            AddUseCtrl(historyTransportView.UseCtrlId, historyTransportView);
            _mainMenuItem.Add(new MenuItem(historyTransportView.UseCtrlId, PackIconKind.Recycle));


            SetShowRenderControl(hotOrderView.UseCtrlId);

            ListBoxMenuMin.ItemsSource = _mainMenuItem;
            ListBoxMenu.ItemsSource = _mainMenuItem;
        }
        private string _curShowCtrlId = string.Empty;
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
                    DoubleAnimation dY = new DoubleAnimation(0, -DockPanelMain.ActualWidth, TimeSpan.FromMilliseconds(300));
                    TranslateTransform tt = new TranslateTransform();
                    DockPanelMain.RenderTransform = tt;
                    dOpacity.Completed += delegate
                    {
                        try
                        {
                            DoubleAnimation dOpacity2 = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                            DoubleAnimation dY2 = new DoubleAnimation(DockPanelMain.ActualWidth, 0, TimeSpan.FromMilliseconds(300));
                            TranslateTransform tt2 = new TranslateTransform();
                            DockPanelMain.RenderTransform = tt2;
                            tt2.BeginAnimation(TranslateTransform.XProperty, dY2);
                            DockPanelMain.BeginAnimation(OpacityProperty, dOpacity2);
                            ChangeShow(useCtrlId);
                            _curShowCtrlId = useCtrlId;
                        }
                        catch (Exception ex)
                        {
                            MessageBoxEx.Show(ex.Message, "错误");
                        }
                    };
                    DockPanelMain.BeginAnimation(OpacityProperty, dOpacity);
                    tt.BeginAnimation(TranslateTransform.XProperty, dY);
                    MenuToggleButton.IsChecked = false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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


        private string _useCtrlId = "指令管理";

        public string UseCtrlId
        {
            get { return _useCtrlId; }
            set { _useCtrlId = value; }

        }


        private void AddUseCtrl(string userCtrlId, UserControl userCtrl)
        {
            if (!_dicUserCtrl.ContainsKey(userCtrlId))
            {
                _dicUserCtrl.Add(userCtrlId, userCtrl);
            }
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
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

        private bool isloaded = false;

        private void TaskManageView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (isloaded) return;
            this.Dispatcher.InvokeAsync(InitAllView, DispatcherPriority.Background);
            isloaded = true;
        }
    }
}
