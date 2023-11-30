using System;
using System.Windows;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{

    public class ExOrderDetailViewModel : ViewModelBase
    {
        private readonly ITransportManage _transportManage;

        private readonly OrderManage _orderManage;
        public ExOrderSingalViewModel ExOrderDataModel { get; set; }
        public ExOrderDetailViewModel(ExOrder exOrder)
        {
            _transportManage = DependencyHelper.GetService<ITransportManage>();
            _orderManage  = DependencyHelper.GetService<OrderManage>();
            SingalViewExOrder = exOrder.DeepClone();
            ExOrderDataModel = new ExOrderSingalViewModel(SingalViewExOrder);
            InitTransportMessageViewModel();
        }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }

        private void InitTransportMessageViewModel()
        {
            TransportMessageViewModel = new TransportMessageViewModel();
            TransportMessageViewModel.UnfinishedTransportList = _transportManage.GetAllTransportMessageByExOrderId(SingalViewExOrder.OrderId).DataPool;
        }

        public TransportMessageViewModel TransportMessageViewModel { get; private set; }

        public ExOrder SingalViewExOrder { get; set; }

        private RelayCommand _editExOrderCommand;

        public RelayCommand EditExOrderCommand
        {
            get
            {
                if (_editExOrderCommand == null)
                {
                    _editExOrderCommand = new RelayCommand(EditExOrder);
                }
                return _editExOrderCommand;
            }
        }

        private RelayCommand _forceFinishOrderCommand;

        public RelayCommand ForceFinishOrderCommand
        {
            get
            {
                if (_forceFinishOrderCommand == null)
                {
                    _forceFinishOrderCommand = new RelayCommand(ForceFinishOrder);
                }
                return _forceFinishOrderCommand;
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        private void EditExOrder()
        {
            try
            {
                if (SingalViewExOrder == null) return;
                MessageBoxResult rt = MessageBoxEx.Show("您确认修改？", "警告", MessageBoxButton.YesNo);

                if (rt.Equals(MessageBoxResult.Yes))
                {
                    //修改真实处理命令
                    OperateResult opResult = _orderManage.UpdateOrder(SingalViewExOrder);
                    SnackbarQueue.MessageQueue.Enqueue("指令修改:" + opResult.Message);

                    string recordMsg = string.Format("修改指令：{0} 结果：{1}", SingalViewExOrder,opResult.Message);
                    OperateLogHelper.Record(recordMsg);

                }
            }
            catch (Exception ex)
            {
                string recordMsg = string.Format("修改指令：{0} 结果：失败", SingalViewExOrder);
                OperateLogHelper.Record(recordMsg);
                MessageBoxEx.Show("指令修改失败:" + ex.ToString());
            }
        }

        /// <summary>
        /// 强制完成数据
        /// </summary>
        private void ForceFinishOrder()
        {
            try
            {
                if (SingalViewExOrder == null) return;
                MessageBoxResult rt = MessageBoxEx.Show("您确认强制完成？", "警告", MessageBoxButton.YesNo);

                if (rt.Equals(MessageBoxResult.Yes))
                {
                    //修改真实处理命令
                    DeviceName deviceName = new DeviceName("Station#9999");
                    OperateResult opResult = _orderManage.UpdateOrderStatus(deviceName, SingalViewExOrder, TaskHandleResultEnum.Finish);
                    SnackbarQueue.MessageQueue.Enqueue("指令强制完成:" + opResult.Message);
                    string recordMsg = string.Format("强制完成指令：{0} 结果：失败", SingalViewExOrder);
                    OperateLogHelper.Record(recordMsg);

                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("指令修改失败:" + ex.ToString());
            }
        }

    }
}
