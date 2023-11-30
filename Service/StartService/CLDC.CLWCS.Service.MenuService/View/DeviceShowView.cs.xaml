using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Domain;
using CLDC.Infrastructrue.UserCtrl.Model;
using MaterialDesignThemes.Wpf;
using MenuItem = CLDC.Infrastructrue.UserCtrl.Model.MenuItem;


namespace CLDC.CLWCS.Service.MenuService.View
{
    /// <summary>
    /// DeviceConfig.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceShowView : IMainUseCtrl
    {
        readonly Dictionary<DeviceTypeEnum, List<ViewAbstract>> _deviceViewDataPool = new Dictionary<DeviceTypeEnum, List<ViewAbstract>>();

        private readonly ObservableCollection<MenuItem> _mainMenuItem = new ObservableCollection<MenuItem>();

        /// <summary>
        /// 加载所有的设备
        /// </summary>
        private void InitizeAllDevice()
        {
            WppnlDevice.Children.Clear();
            List<DeviceBaseAbstract> deviceBaseAbstractLst = DeviceManage.Instance.GetAllData(d => d.IsShowUi.Equals(true));
            foreach (DeviceBaseAbstract device in deviceBaseAbstractLst)
            {
                OperateResult<ViewAbstract> deviceViewResult = device.GetViewForWpf();
                if (!deviceViewResult.IsSuccess)
                {
                    string msg = string.Format("工作者：{0} 获取到的显示界面出错", device + device.Name);
                    MessageBoxEx.Show(msg);
                }
                ViewAbstract deviceView = deviceViewResult.Content;
                AddDeviceViewDic(device.DeviceType, deviceView);
            }
        }

        private void InitMenuItem()
        {
            lock (_deviceViewDataPool)
            {
                foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValuePair in _deviceViewDataPool)
                {
                    PackIconKind icon = PackIconKind.AboutCircle;
                    switch (keyValuePair.Key)
                    {
                        case DeviceTypeEnum.LoadDevice:
                            icon = PackIconKind.BorderBottomVariant;
                            break;
                        case DeviceTypeEnum.IdentityDevice:
                            icon = PackIconKind.Telescope;
                            break;
                        case DeviceTypeEnum.DisplayDevice:
                            icon = PackIconKind.MonitorDashboard;
                            break;
                        case DeviceTypeEnum.TransportDevice:
                            icon = PackIconKind.LorryDelivery;
                            break;
                        case DeviceTypeEnum.RackPlace:
                            icon = PackIconKind.ArrowSplitVertical;
                            break;
                        case DeviceTypeEnum.SwitchDevice:
                            icon = PackIconKind.MotorOutline;
                            break;
                        case DeviceTypeEnum.PalletizerDevice:
                            icon = PackIconKind.Buffer;
                            break;
                    }
                    _mainMenuItem.Add(new MenuItem(keyValuePair.Key.GetDescription(), icon, keyValuePair.Key));
                }
            }
            ListBoxMenuMin.ItemsSource = _mainMenuItem;
            ListBoxMenu.ItemsSource = _mainMenuItem;
        }


        private void AddDeviceViewDic(DeviceTypeEnum deviceType, ViewAbstract deviceView)
        {
            lock (_deviceViewDataPool)
            {
                if (_deviceViewDataPool.ContainsKey(deviceType))
                {
                    _deviceViewDataPool[deviceType].Add(deviceView);
                }
                else
                {
                    List<ViewAbstract> newViewList = new List<ViewAbstract>() { deviceView };
                    _deviceViewDataPool.Add(deviceType, newViewList);
                }
                WppnlDevice.Children.Add(deviceView);
            }
        }

        public DeviceShowView()
        {
            InitializeComponent();

        }


        /// <summary>
        /// 更新大小
        /// </summary>
        /// <param name="tempUCStationLst">显示的对象 ，为空则调整现有的对象大小wppnlDevice</param>
        public void DisPlayUpdateData()
        {
            int width = 0; //显示总宽度
            int minWidth = 350; //窗体最小宽度
            int newWidth = 0; //窗体 计算的新宽度
            this.WppnlDevice.Dispatcher.Invoke(new Action(delegate
            {
                width = Convert.ToInt32(WppnlDevice.ActualWidth);

                int count = width / minWidth;
                if (count >= 5)
                {
                    if (WppnlDevice.Children.Count <= 3)
                    {
                        count = WppnlDevice.Children.Count == 0 ? 3 : WppnlDevice.Children.Count; //最大显示4个
                    }
                    else
                    {
                        count = 4; //最大显示4个
                    }
                }
                newWidth = (count == 0 ? 0 : (minWidth + (width - minWidth * count) / count));
                foreach (var loadStation in WppnlDevice.Children)
                {
                    var nvcDevice = loadStation as UserControl;
                    if (width > 0 && newWidth >= minWidth)
                    {
                        nvcDevice.Width = newWidth;
                    }
                }
            }), DispatcherPriority.Background);
        }

        #region 设备面板切换(工作、任务、日志)

        /// <summary>
        /// 切换设备工作状态面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowWorkStatus_Click(object sender, RoutedEventArgs e)
        {
            ChangeWorkerCardState(ShowCardShowEnum.State);
        }



        /// <summary>
        /// 切换任务工作状态面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowTaskDatas_Click(object sender, RoutedEventArgs e)
        {
            ChangeWorkerCardState(ShowCardShowEnum.Task);
        }

        /// <summary>
        /// 切换设备日志状态面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowLogInfos_Click(object sender, RoutedEventArgs e)
        {
            ChangeWorkerCardState(ShowCardShowEnum.Log);
        }


        /// <summary>
        /// tablNodeInfo  面板整体切换
        /// </summary>
        /// <param name="state"></param>
        private void ChangeWorkerCardState(ShowCardShowEnum state)
        {
            foreach (object child in WppnlDevice.Children)
            {
                IShowCard card = child as IShowCard;
                if (card != null)
                {
                    card.ChangeViewState(state);
                }
            }
        }

        #endregion

        /// <summary>
        /// 检测用户 输入的查询条件 是否是通过ID查询，否则 取工位名称
        /// </summary>
        /// <param name="strInputParms"></param>
        /// <returns></returns>
        private bool IsUsedSearchById(string strInputParms)
        {
            bool isOk = false;
            try
            {
                int value = int.Parse(strInputParms);
                isOk = true;
            }
            catch (Exception ex)
            {
                //输入非ID
            }
            return isOk;
        }

        private void UserControlScrol(ViewAbstract deviceView)
        {
            var currentScrollPosition = Scroll.VerticalOffset;
            var point = new Point(0, currentScrollPosition);
            var targetPosition = deviceView.TransformToVisual(Scroll).Transform(point);
            Scroll.ScrollToVerticalOffset(targetPosition.Y);

            deviceView.ViewModel.IsChangedColor = false;
            deviceView.ViewModel.IsChangedColor = true;
        }

        #region  查询节点对象  根据节点名称和ID(模糊查询)

        /// <summary>
        ///  查询节点对象  根据节点名称和ID(模糊查询)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchUCStation_Click(object sender, RoutedEventArgs e)
        {
            DataSearchMehod();
        }

        private void DataSearchMehod()
        {
            string strInputData = TxtInputStationInfo.Text.Trim();

            if (string.IsNullOrEmpty(strInputData)) return;

            if (ValidData.CheckSpecialCharacters(TxtInputStationInfo.Text))
            {
                SnackbarQueue.Enqueue(string.Format("禁止非法输入：{0} ", TxtInputStationInfo.Text));
                return;
            }
            if (_deviceViewDataPool.Count == 0) return;
            bool isUsedIdSearch = IsUsedSearchById(strInputData);

            lock (_deviceViewDataPool)
            {
                foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValuePair in _deviceViewDataPool)
                {
                    foreach (ViewAbstract deviceView in keyValuePair.Value)
                    {
                        if (isUsedIdSearch)
                        {
                            //ID查询
                            int id = int.Parse(strInputData);
                            if (deviceView != null && deviceView.ViewModel != null && id == deviceView.ViewModel.Id)
                            {
                                UserControlScrol(deviceView);
                                return;
                            }
                        }
                        else
                        {
                            //工位名称查询
                            if (deviceView != null && deviceView.ViewModel != null && Name == deviceView.ViewModel.Name)
                            {
                                UserControlScrol(deviceView);
                                return;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void txtInputStationInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            DataSearchMehod();
        }

        private void BtnClearInputText_Click(object sender, RoutedEventArgs e)
        {
            TxtInputStationInfo.Text = "";
        }

        private void ContentDockpanel_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DisPlayUpdateData();
        }

        private string _useCtrlId = "设备管理";

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

        private void FilterDeviceView(object where)
        {
            if (where == null)
            {
                lock (_deviceViewDataPool)
                {
                    foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValue in _deviceViewDataPool)
                    {
                        foreach (ViewAbstract deviceView in keyValue.Value)
                        {
                            deviceView.Show();
                        }
                    }
                }
                return;
            }
            DeviceTypeEnum deviceType = (DeviceTypeEnum)where;
            lock (_deviceViewDataPool)
            {
                foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValue in _deviceViewDataPool)
                {
                    if (keyValue.Key.Equals(deviceType))
                    {
                        keyValue.Value.ForEach(d => d.Show());
                    }
                    else
                    {
                        keyValue.Value.ForEach(d => d.Hide());
                    }
                }
            }

        }

        private void BtnShowAll_OnClick(object sender, RoutedEventArgs e)
        {
            FilterDeviceView(null);
            DisPlayUpdateData();
            MenuToggleButton.IsChecked = false;

        }

        private void ListBoxMenuMin_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb == null)
            {
                return;
            }
            MenuItem selectedItem = (MenuItem)lb.SelectedValue;
            FilterDeviceView(selectedItem.Tag);
            DisPlayUpdateData();
            MenuToggleButton.IsChecked = false;
        }

        private void ShowHasTask()
        {
            lock (_deviceViewDataPool)
            {
                foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValue in _deviceViewDataPool)
                {
                    keyValue.Value.ForEach(v =>
                    {
                        if (v.IsHasAnyTask())
                        {
                            v.Show();
                        }
                        else
                        {
                            v.Hide();
                        }
                    });
                }
            }
        }

        private void ShowHasError()
        {
            lock (_deviceViewDataPool)
            {
                foreach (KeyValuePair<DeviceTypeEnum, List<ViewAbstract>> keyValue in _deviceViewDataPool)
                {
                    keyValue.Value.ForEach(v =>
                    {
                        if (v.IsHasError())
                        {
                            v.Show();
                        }
                        else
                        {
                            v.Hide();
                        }
                    });
                }
            }
        }

        private void CbHasTask_OnChecked(object sender, RoutedEventArgs e)
        {
            ShowHasTask();
        }

        private void CbHasTask_OnUnchecked(object sender, RoutedEventArgs e)
        {
            FilterDeviceView(null);
            DisPlayUpdateData();
        }

        private void CbError_OnChecked(object sender, RoutedEventArgs e)
        {
            ShowHasError();
        }

        private void CbError_OnUnchecked(object sender, RoutedEventArgs e)
        {
            FilterDeviceView(null);
            DisPlayUpdateData();
        }

        private bool isLoaded = false;
        private void DeviceShowView_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (isLoaded == false)
                {
                    InitizeAllDevice();
                    InitMenuItem();
                    isLoaded = true;
                }
                DisPlayUpdateData();
            }, DispatcherPriority.DataBind);
        }
    }
}
