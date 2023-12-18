using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.SqlSugar;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateReturnOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateReturnOrderView : UserControl
    {
        private WmsDataService _wmsDataService;
        private readonly IOrderSNGenerate _orderGeneraterHandler;

        public CreateReturnOrderView()
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            _orderGeneraterHandler = DependencyHelper.GetService<OrderSNGenerate>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //InitCbArea(); InitCbShelf();
            InitCbReason();
            CbReason.SelectedIndex = 1;
            DataContext = CreateOrderViewModel.SingleInstance;
            CreateOrderViewModel.SingleInstance.BarcodeList.Clear();
            CreateOrderViewModel.SingleInstance.BarcodeCount = "0";
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            //先停止扫描再保存数据
            CreateOrderViewModel.SingleInstance.StopCommand.Execute(null);
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择或填写事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (BarcodeGrid.Items.Count == 0)
            {
                MessageBoxEx.Show("未识别到装备标签,请重新扫描!");
                return;
            }
            progressLoop.Visibility = Visibility.Hidden;

            long orderId = _orderGeneraterHandler.GetGlobalNewTaskId();
            string orderSN = "In_" + orderId;
            List<OrderDetail> detailList = new List<OrderDetail>();
            List<string> barcodes = new List<string>();
            foreach (var item in BarcodeGrid.Items)
            {
                string barcode = (BarcodeGrid.Columns[2].GetCellContent(item) as TextBlock).Text;
                Inventory inventory = _wmsDataService.GetInventory(barcode);
                if (inventory == null)
                {
                    MessageBoxEx.Show("查询" + barcode + "对应的库存失败!", "错误", MessageBoxButton.OK);
                    return;
                }
                barcodes.Add(inventory.Barcode);                

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderSN = orderSN;
                orderDetail.Barcode = barcode;
                orderDetail.Remark = string.Empty;
                orderDetail.IsDeleted = false;
                orderDetail.UnitName = inventory.UnitName;
                orderDetail.MaterialCode = inventory.MaterialCode;
                orderDetail.MaterialDesc = inventory.MaterialDesc;
                orderDetail.ShelfCode = lbShelfCode.Content.ToString() ;
                orderDetail.ShelfName = lbShelf.Content.ToString();
                detailList.Add(orderDetail);
            }
            OperateResult updateInvResult = _wmsDataService.UpdateInventory((int)InvStatusEnum.在库, barcodes);
            if (!updateInvResult.IsSuccess)
            {
                MessageBoxEx.Show("更新库存状态失败，原因：" + updateInvResult.Message, "错误", MessageBoxButton.OK);
                return;
            }
            OperateResult createDetailResult = _wmsDataService.CreateNewOrderDetail(detailList);
            if (!createDetailResult.IsSuccess)
            {
                MessageBoxEx.Show("创建入库明细失败，原因：" + createDetailResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            Order newOrder = new Order();
            newOrder.Id = orderId;
            newOrder.InOutType = InOrOutEnum.入库;
            newOrder.AreaCode = lbAreaCode.Content.ToString();
            newOrder.AreaName = lbArea.Content.ToString();
            newOrder.CreatedTime = DateTime.Now;
            newOrder.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
            newOrder.IsDeleted = false;
            newOrder.Reason = (string)CbReason.Text;
            newOrder.OrderSN = orderSN;
            newOrder.Qty = BarcodeGrid.Items.Count;
            newOrder.Status = InvStatusEnum.在库;

            OperateResult createResult = _wmsDataService.CreateNewInOrder(newOrder);
            if (!createResult.IsSuccess)
            {
                MessageBoxEx.Show("创建装备归还失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            SnackbarQueue.MessageQueue.Enqueue("创建装备归还成功");
            DialogHost.CloseDialogCommand.Execute(createResult.Message, this);
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择出入事由", "提示", MessageBoxButton.OK);
                return;
            }
            CreateOrderViewModel.SingleInstance.ScanCommand.Execute(null);
            progressLoop.Visibility = Visibility.Visible;
        }

        private readonly Dictionary<TaskTypeEnum, string> _taskTypeDict = new Dictionary<TaskTypeEnum, string>();
        public Dictionary<TaskTypeEnum, string> TaskTypeDict
        {
            get
            {
                if (_taskTypeDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(TaskTypeEnum)))
                    {
                        TaskTypeEnum em = (TaskTypeEnum)value;
                        _taskTypeDict.Add(em, em.GetDescription());
                    }
                }
                return _taskTypeDict;
            }
        }

        private void InitCbReason()
        {
            CbReason.SelectedValuePath = "Id";
            CbReason.DisplayMemberPath = "Name";
            CbReason.ItemsSource = ReasonConfig.Instance.ReasonList.Where(x=>x.Type=="入库");
        }

        //private void InitCbArea()
        //{
        //    CbArea.SelectedValuePath = "AreaCode";
        //    CbArea.DisplayMemberPath = "AreaName";
        //    CbArea.ItemsSource = _wmsDataService.GetAreaList(string.Empty);
        //}

        //private void InitCbShelf()
        //{
        //    CbShelf.SelectedValuePath = "Code";
        //    CbShelf.DisplayMemberPath = "Name";
        //    CbShelf.ItemsSource = _wmsDataService.GetShelfList((string)CbArea.SelectedValue);
        //}

        //private void CbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    InitCbShelf();
        //}

    }
}
