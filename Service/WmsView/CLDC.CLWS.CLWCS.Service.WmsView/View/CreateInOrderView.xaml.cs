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
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateInOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateInOrderView : UserControl
    {
        private WmsDataService _wmsDataService;
        private readonly IOrderSNGenerate _orderGeneraterHandler;

        public CreateInOrderView()
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            _orderGeneraterHandler = DependencyHelper.GetService<OrderSNGenerate>();
            InitCbTaskType();
            InitCbMaterialDesc();
            InitCbUnit();
            InitCbArea();
            InitCbReason();
            InitCbShelf();
            DataContext = CreateInOrderViewModel.SingleInstance;
            //CreateInOrderViewModel.SingleInstance.BarcodeList.Clear();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //先停止扫描再保存数据
            CreateInOrderViewModel.SingleInstance.StopCommand.Execute(null);

            if (CbTaskType.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择任务分类", "提示", MessageBoxButton.OK);
                return;
            }
            if(CbReason.SelectedIndex==-1 && string.IsNullOrEmpty(CbReason.Text))
            {
                MessageBoxEx.Show("请选择或填写事由", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbArea.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择存放区域", "提示", MessageBoxButton.OK);
                return;
            }
            if (CbMaterialDesc.SelectedIndex == -1 && string.IsNullOrEmpty(CbMaterialDesc.Text))
            {
                MessageBoxEx.Show("请选择或填写装备", "提示", MessageBoxButton.OK);
                return;
            }
            if(CbUnit.SelectedIndex==-1)
            {
                MessageBoxEx.Show("请选择单位", "提示", MessageBoxButton.OK);
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
            if (BarcodeGrid.Items.Count - 1 != int.Parse(TbQty.Text))
            {
                MessageBoxEx.Show("识别到装备标签的数量与入库数量不一致,请重新扫描!");
                return;
            }

            long orderId= _orderGeneraterHandler.GetGlobalNewTaskId();
            string orderSN = "In_" + orderId;
            foreach (var item in BarcodeGrid.Items)
            {
                InOrderDetail orderDetail = new InOrderDetail();
                orderDetail.OrderSN = orderSN;
                orderDetail.Barcode = (BarcodeGrid.Columns[1].GetCellContent(item) as TextBlock).Text;
                orderDetail.Remark = string.Empty;
                orderDetail.IsDeleted = false;
                OperateResult createDetailResult = _wmsDataService.CreateNewOrderDetail(orderDetail);
                if (!createDetailResult.IsSuccess)
                {
                    MessageBoxEx.Show("创建入库单明细失败，原因：" + createDetailResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }

                Inventory inventory = new Inventory();
                inventory.AreaCode= (string)CbArea.SelectedValue;
                inventory.AreaName = (string)CbArea.Text;
                inventory.CreatedTime = DateTime.Now;
                inventory.CreatedUserId = CookieService.CurSession.UserInfo.Account.AccId;
                inventory.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
                inventory.IsDeleted = false;
                inventory.MaterialCategory = 4;// (int)CbArea.SelectedValue;
                inventory.MaterialCode = (string)CbMaterialDesc.SelectedValue;
                inventory.MaterialDesc = (string)CbMaterialDesc.Text;
                inventory.Reason = (string)CbReason.Text;
                inventory.ShelfCode = (string)CbShelf.SelectedValue;
                inventory.ShelfName = (string)CbShelf.Text;
                inventory.OrderSN = orderSN;
                inventory.Qty = int.Parse(TbQty.Text);
                inventory.Status = (int)InvStatusEnum.在库;
                inventory.TaskType = (int)CbTaskType.SelectedValue;
                inventory.UnitId = (long)CbUnit.SelectedValue;
                inventory.UnitName = (string)CbUnit.Text;
                inventory.Barcode= (BarcodeGrid.Columns[1].GetCellContent(item) as TextBlock).Text;
                OperateResult createInventoryResult = _wmsDataService.CreateNewInventory(inventory);
                if (!createInventoryResult.IsSuccess)
                {
                    MessageBoxEx.Show("创建库存失败，原因：" + createInventoryResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }
            }

            InOrder newOrder = new InOrder();
            newOrder.Id = orderId;
            newOrder.AreaCode = (string)CbArea.SelectedValue;
            newOrder.AreaName = (string)CbArea.Text;
            newOrder.CreatedTime = DateTime.Now;
            newOrder.CreatedUserId = CookieService.CurSession.UserInfo.Account.AccId;
            newOrder.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
            newOrder.IsDeleted = false;
            newOrder.MaterialCategory = 4;// (int)CbArea.SelectedValue;
            newOrder.MaterialCode = (string)CbMaterialDesc.SelectedValue;
            newOrder.MaterialDesc = (string)CbMaterialDesc.Text;
            newOrder.Reason = (string)CbReason.Text;
            newOrder.ShelfCode = (string)CbShelf.SelectedValue;
            newOrder.ShelfName = (string)CbShelf.Text;
            newOrder.OrderSN = orderSN;
            newOrder.Qty = int.Parse(TbQty.Text);
            newOrder.Status = 0;
            newOrder.TaskType = (int)CbTaskType.SelectedValue;
            newOrder.UnitId = (long)CbUnit.SelectedValue;
            newOrder.UnitName = (string)CbUnit.Text;

            OperateResult createResult = _wmsDataService.CreateNewInOrder(newOrder);
            if (!createResult.IsSuccess)
            {
                MessageBoxEx.Show("创建新购入库失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                return;
            }

            SnackbarQueue.MessageQueue.Enqueue("创建新购入库成功");
            DialogHost.CloseDialogCommand.Execute(createResult.Message, this);
        }

        private void BtnScanRfid_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CbMaterialDesc_TextChanged(object sender, RoutedEventArgs e)
        {
            CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList(CbMaterialDesc.Text);
            CbMaterialDesc.IsDropDownOpen = true;
        }

        private void CbReason_TextChanged(object sender, RoutedEventArgs e)
        {
            CbReason.ItemsSource = ReasonDict.Where(x=>x.Value.Contains(CbReason.Text));
            CbReason.IsDropDownOpen = true;
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

        private readonly Dictionary<ReasonEnum, string> _reasonEnumDict = new Dictionary<ReasonEnum, string>();
        public Dictionary<ReasonEnum, string> ReasonDict
        {
            get
            {
                if (_reasonEnumDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(ReasonEnum)))
                    {
                        ReasonEnum em = (ReasonEnum)value;
                        _reasonEnumDict.Add(em, em.GetDescription());
                    }
                }
                return _reasonEnumDict;
            }
        }

        private void InitCbReason()
        {
            CbReason.SelectedValuePath = "Key";
            CbReason.DisplayMemberPath = "Value";
            CbReason.ItemsSource = ReasonDict;
        }

        private void InitCbTaskType()
        {
            CbTaskType.SelectedValuePath = "Key";
            CbTaskType.DisplayMemberPath = "Value";
            CbTaskType.ItemsSource = TaskTypeDict;
        }

        private void InitCbMaterialDesc()
        {
            CbMaterialDesc.SelectedValuePath = "MaterialCode";
            CbMaterialDesc.DisplayMemberPath = "MaterialDesc";
            CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList(string.Empty);
        }

        private void InitCbUnit()
        {
            CbUnit.SelectedValuePath = "UnitId";
            CbUnit.DisplayMemberPath = "UnitName";
            CbUnit.ItemsSource = _wmsDataService.GetUnitList(string.Empty);
        }

        private void InitCbArea()
        {
            CbArea.SelectedValuePath = "Code";
            CbArea.DisplayMemberPath = "Name";
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
