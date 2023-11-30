using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService;
using Newtonsoft.Json;


namespace CLWCS.WareHouse.Worker.HeFei
{
    public class XYZABBTransportPointStationWorker2 : OrderWorkerAbstract<XYZABBWorkerBusiness2>
    {

        protected override OperateResult ParticularStart()
        {
            OperateResult baseStart = base.ParticularStart();
            if (!baseStart.IsSuccess)
            {
                return baseStart;
            }

            OperateResult registerResult = RegisterHandler();
            if (!registerResult.IsSuccess)
            {
                return registerResult;
            }
            return OperateResult.CreateSuccessResult();
        }
        public OperateResult RegisterHandler()
        {
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            OperateResult result = WorkerBusiness.DeviceExceptionHandle(exceptionMsg);
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



        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            OrderWorkerViewModel<XYZABBWorkerBusiness2> viewModel = new OrderWorkerViewModel<XYZABBWorkerBusiness2>(this);
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
