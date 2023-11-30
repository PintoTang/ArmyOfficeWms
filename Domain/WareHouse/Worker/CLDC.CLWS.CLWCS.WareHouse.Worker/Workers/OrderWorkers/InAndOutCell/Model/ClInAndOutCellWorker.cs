using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    /// <summary>
    /// 出入库工作者
    /// </summary>
    public class ClInAndOutCellWorker : InAndOutCellWorkerAbstract
    {
        /// <summary>
        /// 设备异常搬运处理
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>OperateResult</returns>
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> type)
        {
            OperateResult result = WorkerBusiness.DeviceExceptionHandle(type);
            if (result.IsSuccess)
            {
                LogMessage(result.Message, EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(result.Message, EnumLogLevel.Error, true);
            }
            return result;
        }




        /// <summary>
        /// viewModel
        /// </summary>
        /// <returns>OperateResult<WareHouseViewModelBase></returns>
        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            InAndOutWorkerViewModel viewModel = new InAndOutWorkerViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;

        }
        /// <summary>
        /// view
        /// </summary>
        /// <returns>OperateResult<ViewAbstract></returns>
        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            WorkerShowCard view = new WorkerShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }
        /// <summary>
        /// DetailView
        /// </summary>
        /// <returns> OperateResult<UserControl></returns>
        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            OrderWorkerDetailView view = new OrderWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }
        /// <summary>
        /// MonitorView
        /// </summary>
        /// <returns>OperateResult<UserControl></returns>
        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view=new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        
    }
}
