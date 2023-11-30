using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
	/// <summary>
	/// 科陆辊筒线工作者虚类
	/// </summary>
    public class ClRollerWorker : OrderWorkerAbstract<OrderWorkerBuinessAbstract>
	{
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> type)
		{
			return OperateResult.CreateSuccessResult();
		}



        protected  override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            OrderWorkerViewModel<OrderWorkerBuinessAbstract> viewModel = new OrderWorkerViewModel<OrderWorkerBuinessAbstract>(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
           
        }



        protected override OperateResult<ViewAbstract> CreateView()
	    {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            WorkerShowCard view = new WorkerShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
	    }

	    protected override OperateResult<UserControl> CreateDetailView()
	    {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            OrderWorkerDetailView view = new OrderWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
	    }

	    protected override OperateResult<UserControl> CreateMonitorView()
	    {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
	    }
	}
}
