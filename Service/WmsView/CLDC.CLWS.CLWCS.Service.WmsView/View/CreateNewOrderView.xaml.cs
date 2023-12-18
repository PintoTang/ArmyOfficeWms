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
    /// CreateNewOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateNewOrderView : UserControl
    {
        private WmsDataService _wmsDataService;
        private readonly IOrderSNGenerate _orderGeneraterHandler;
        private string reason = string.Empty;

        public CreateNewOrderView(string _reason)
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            _orderGeneraterHandler = DependencyHelper.GetService<OrderSNGenerate>();
            this.reason = _reason;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitCbMaterialDesc(); InitCbTeam();
            InitCbArea(); InitCbReason(); InitCbShelf();
            if (reason == "2")
                lbTitle.Content = "调拨入库";
            else
                lbTitle.Content = "首次入库";
            CbReason.SelectedIndex = int.Parse(reason);
            DataContext = CreateOrderViewModel.SingleInstance;
            //CreateOrderViewModel.SingleInstance.BarcodeList.Clear();
            CreateOrderViewModel.SingleInstance.BarcodeCount = "0";
        }

        private void CbMaterialDesc_TextChanged(object sender, RoutedEventArgs e)
        {
            //CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList(CbMaterialDesc.Text);
            //CbMaterialDesc.IsDropDownOpen = true;
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
            CbReason.ItemsSource = ReasonConfig.Instance.ReasonList.Where(x => x.Type == "入库");
        }

        private void InitCbMaterialDesc()
        {
            CbMaterialDesc.SelectedValuePath = "MaterialCode";
            CbMaterialDesc.DisplayMemberPath = "MaterialDesc";
            CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList((string)CbArea.SelectedValue);
        }

        private void InitCbArea()
        {
            CbArea.SelectedValuePath = "AreaCode";
            CbArea.DisplayMemberPath = "AreaName";
            CbArea.ItemsSource = _wmsDataService.GetAreaList(string.Empty);
        }

        private void InitCbTeam()
        {
            List<AreaTeam> list = new List<AreaTeam>();
            CbTeam.SelectedValuePath = "Name";
            CbTeam.DisplayMemberPath = "Name";
            for (int i = 1; i < 4; i++)
            {
                AreaTeam team = new AreaTeam();
                team.Id = i; team.Name = i + "排"; team.Remark = string.Empty;
                list.Add(team);
            }
            list.Add(new AreaTeam { Id = 4, Name = "首长机关" });
            list.Add(new AreaTeam { Id = 5, Name = "民兵" });
            CbTeam.ItemsSource = list;
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
            InitCbMaterialDesc();
        }

        private void CbMaterialDesc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = _wmsDataService.GetMaterial((string)CbMaterialDesc.SelectedValue);
            if (model == null)
            {
                tbUnitName.Text = string.Empty;
            }
            else
            {
                tbUnitName.Text = model.UnitName;
            }
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (CbArea.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分类", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbTeam.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分队", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbMaterialDesc.SelectedIndex == -1 && string.IsNullOrEmpty(CbMaterialDesc.Text))
            {
                MessageBoxEx.Show("请选择装备", "提示", MessageBoxButton.OK);
                return;
            }
            if (!ValidData.CheckData(TbQty.Text, "^[1-9]\\d*$"))
            {
                MessageBoxEx.Show("请输入正整数!");
                return;
            }
            CreateOrderViewModel.SingleInstance.ScanCommand.Execute(null);
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
            if (CbTeam.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分队", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbReason.SelectedIndex == -1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbMaterialDesc.SelectedIndex == -1 && string.IsNullOrEmpty(CbMaterialDesc.Text))
            {
                MessageBoxEx.Show("请选择装备", "提示", MessageBoxButton.OK);
                return;
            }
            if (!ValidData.CheckData(TbQty.Text, "^[1-9]\\d*$"))
            {
                MessageBoxEx.Show("请输入正整数!");
                return;
            }
            if (BarcodeGrid.Items.Count == 0)
            {
                MessageBoxEx.Show("未识别到装备标签,请重新扫描!");
                return;
            }
            if (BarcodeGrid.Items.Count != int.Parse(TbQty.Text))
            {
                MessageBoxEx.Show("识别到装备标签的数量与入库数量不一致,请重新扫描!");
                return;
            }

            long orderId = _orderGeneraterHandler.GetGlobalNewTaskId();
            string orderSN = "In_" + orderId;

            List<OrderDetail> detailList = new List<OrderDetail>();
            List<Inventory> inventoryList = new List<Inventory>();
            foreach (var item in BarcodeGrid.ItemsSource)
            {
                RfidBarcode barcodeItem = item as RfidBarcode;
                Inventory invExist = _wmsDataService.GetInventory(barcodeItem.Barcode);
                if (invExist != null && invExist.Status == InvStatusEnum.在库)
                {
                    MessageBoxEx.Show("装备标签：" + invExist.Barcode + "是在库的状态，请检查!", "错误", MessageBoxButton.OK);
                    return;
                }

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderSN = orderSN;
                //DataGridTextColumn barColumn = this.BarcodeGrid.Columns[1] as DataGridTextColumn;
                //FrameworkElement cellData = barColumn.GetCellContent(this.BarcodeGrid.Items[i]);
                //if (cellData != null)
                orderDetail.Barcode = barcodeItem.Barcode;
                orderDetail.Remark = string.Empty;
                orderDetail.IsDeleted = false;
                orderDetail.UnitName = tbUnitName.Text;
                orderDetail.MaterialCode = (string)CbMaterialDesc.SelectedValue;
                orderDetail.MaterialDesc = (string)CbMaterialDesc.Text;
                orderDetail.ShelfCode = (string)CbShelf.SelectedValue;
                orderDetail.ShelfName = (string)CbShelf.Text;
                detailList.Add(orderDetail);

                Inventory inventory = new Inventory();
                inventory.AreaCode = (string)CbArea.SelectedValue;
                inventory.AreaName = (string)CbArea.Text;
                inventory.CreatedTime = DateTime.Now;
                inventory.CreatedUserId = CookieService.CurSession.UserInfo.Account.AccId;
                inventory.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
                inventory.IsDeleted = false;
                inventory.MaterialCode = (string)CbMaterialDesc.SelectedValue;
                inventory.MaterialDesc = (string)CbMaterialDesc.Text;
                inventory.Reason = (string)CbReason.Text;
                inventory.ShelfCode = (string)CbShelf.SelectedValue;
                inventory.ShelfName = (string)CbShelf.Text;
                inventory.OrderSN = orderSN;
                inventory.Qty = 1;
                inventory.Status = InvStatusEnum.在库;
                inventory.UnitName = tbUnitName.Text;
                inventory.Barcode = barcodeItem.Barcode;
                inventory.AreaTeam = (string)CbTeam.SelectedValue;
                inventoryList.Add(inventory);
            }
            OperateResult createDetailResult = _wmsDataService.CreateNewOrderDetail(detailList);
            if (!createDetailResult.IsSuccess)
            {
                MessageBoxEx.Show("创建入库单明细失败，原因：" + createDetailResult.Message, "错误", MessageBoxButton.OK);
                return;
            }
            OperateResult createInventoryResult = _wmsDataService.CreateNewInventory(inventoryList);
            if (!createInventoryResult.IsSuccess)
            {
                MessageBoxEx.Show("创建库存失败，原因：" + createInventoryResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            Order newOrder = new Order();
            newOrder.Id = orderId;
            newOrder.InOutType = InOrOutEnum.入库;
            newOrder.AreaCode = (string)CbArea.SelectedValue;
            newOrder.AreaName = (string)CbArea.Text;
            newOrder.UnitName = tbUnitName.Text;
            newOrder.CreatedTime = DateTime.Now;
            newOrder.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
            newOrder.IsDeleted = false;
            newOrder.Reason = (string)CbReason.Text;
            newOrder.OrderSN = orderSN;
            newOrder.Qty = int.Parse(TbQty.Text);
            newOrder.Status = InvStatusEnum.在库;
            newOrder.AreaTeam = (string)CbTeam.SelectedValue;

            OperateResult createResult = _wmsDataService.CreateNewInOrder(newOrder);
            if (!createResult.IsSuccess)
            {
                MessageBoxEx.Show("创建新购入库失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            SnackbarQueue.MessageQueue.Enqueue("创建新购入库成功");
            DialogHost.CloseDialogCommand.Execute(createResult.Message, this);
        }

    }
}
