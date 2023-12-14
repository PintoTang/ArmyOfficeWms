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
    /// CreateOutOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateOutOrderView : UserControl
    {
        private WmsDataService _wmsDataService;
        private readonly IOrderSNGenerate _orderGeneraterHandler;

        public CreateOutOrderView()
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            _orderGeneraterHandler = DependencyHelper.GetService<OrderSNGenerate>();            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitCbArea(); InitCbReason(); InitCbShelf();
            DataContext = CreateOrderViewModel.SingleInstance;
            //CreateOrderViewModel.SingleInstance.BarcodeList.Clear();
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            //先停止扫描再保存数据
            CreateOrderViewModel.SingleInstance.StopCommand.Execute(null);

            if (CbArea.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分类", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择出入事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (BarcodeGrid.Items.Count == 0)
            {
                MessageBoxEx.Show("未识别到装备标签,请重新扫描!");
                return;
            }

            long orderId = _orderGeneraterHandler.GetGlobalNewTaskId();
            string orderSN = "Out_" + orderId;
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
                orderDetail.ShelfCode = (string)CbShelf.SelectedValue;
                orderDetail.ShelfName = (string)CbShelf.Text;
                detailList.Add(orderDetail);                             
            }
            OperateResult updateInvResult = _wmsDataService.UpdateInventory((int)InvStatusEnum.出库, barcodes);
            if (!updateInvResult.IsSuccess)
            {
                MessageBoxEx.Show("更新库存状态失败，原因：" + updateInvResult.Message, "错误", MessageBoxButton.OK);
                return;
            }
            OperateResult createDetailResult = _wmsDataService.CreateNewOrderDetail(detailList);
            if (!createDetailResult.IsSuccess)
            {
                MessageBoxEx.Show("创建出库明细失败，原因：" + createDetailResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            Order newOrder = new Order();
            newOrder.Id = orderId;
            newOrder.InOutType = InOrOutEnum.出库;
            newOrder.AreaCode = (string)CbArea.SelectedValue;
            newOrder.AreaName = (string)CbArea.Text;
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
                MessageBoxEx.Show("创建装备出库失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            SnackbarQueue.MessageQueue.Enqueue("创建装备出库成功");
            DialogHost.CloseDialogCommand.Execute(createResult.Message, this);
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (CbArea.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分类", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择出入事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (BarcodeGrid.Items.Count == 0)
            {
                MessageBoxEx.Show("未识别到装备标签,请重新扫描!");
                return;
            }
            CreateOrderViewModel.SingleInstance.ScanCommand.Execute(null);
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
            CbReason.ItemsSource = ReasonConfig.Instance.ReasonList.Where(x => x.Type == "出库");
        }

        private void InitCbArea()
        {
            CbArea.SelectedValuePath = "AreaCode";
            CbArea.DisplayMemberPath = "AreaName";
            CbArea.ItemsSource = _wmsDataService.GetAreaList(string.Empty);
        }

        private void InitCbShelf()
        {
            CbShelf.SelectedValuePath = "Code";
            CbShelf.DisplayMemberPath = "Name";
            CbShelf.ItemsSource = _wmsDataService.GetShelfList((string)CbArea.SelectedValue);
        }

        private void CbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitCbShelf();
        }

    }
}
