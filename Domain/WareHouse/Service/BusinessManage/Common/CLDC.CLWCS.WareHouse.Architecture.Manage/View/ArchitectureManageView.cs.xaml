using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWCS.WareHouse.Architecture.Manage
{
    /// <summary>
    /// ArchitectureManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ArchitectureManageView : UserControl,IMainUseCtrl
    {
        public ArchitectureManageView()
        {
            InitializeComponent();
        }

        private string _useCtrlId = "基础数据";

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
        private readonly ObservableCollection<Infrastructrue.UserCtrl.Model.MenuItem> _mainMenuItem = new ObservableCollection<Infrastructrue.UserCtrl.Model.MenuItem>();

        private void AddUseCtrl(string userCtrlId, UserControl userCtrl)
        {
            if (!_dicUserCtrl.ContainsKey(userCtrlId))
            {
                _dicUserCtrl.Add(userCtrlId, userCtrl);
            }
        }

        private void InitAllView()
        {
            if (ArchitectureManage.Instance.GetAllData().Count == 0)
            {
                ToolZone.Visibility = Visibility.Collapsed;
                return;
            }
            foreach (ArchitectureDataAbstract webserviceAbstract in ArchitectureManage.Instance.GetAllData())
            {
                if (!webserviceAbstract.IsShowUi)
                {
                    continue;
                }
                UserControl dbServiceMonitorView = webserviceAbstract.GetDetailView();
                DockPanelMain.Children.Add(dbServiceMonitorView);
                AddUseCtrl(webserviceAbstract.Name, dbServiceMonitorView);
                _mainMenuItem.Add(new Infrastructrue.UserCtrl.Model.MenuItem(webserviceAbstract.Name, webserviceAbstract.IconKind));
            }

            ListBoxMenuMin.ItemsSource = _mainMenuItem;
            ListBoxMenu.ItemsSource = _mainMenuItem;
        }
        private bool isLoaded = false;
        private void ArchitectureManageView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded) return;
            this.Dispatcher.InvokeAsync(InitAllView, DispatcherPriority.Background);
            isLoaded = true;
        }

        private string _curShowCtrlId = string.Empty;

        private static readonly object _lockSwitchView = new object();

        /// <summary>
        /// 管理所有的UserControl对象
        /// </summary>
        private readonly Dictionary<string, UserControl> _dicUserCtrl = new Dictionary<string, UserControl>();
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

        private void SetShowRenderControl(string useCtrlId)
        {
            lock (_lockSwitchView)
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

        private void ListBoxMenuMin_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb == null)
            {
                return;
            }
            Infrastructrue.UserCtrl.Model.MenuItem selectedItem = (Infrastructrue.UserCtrl.Model.MenuItem)lb.SelectedValue;
            if (selectedItem == null)
            {
                return;
            }
            SetShowRenderControl(selectedItem.Title);
        }
    }
}
