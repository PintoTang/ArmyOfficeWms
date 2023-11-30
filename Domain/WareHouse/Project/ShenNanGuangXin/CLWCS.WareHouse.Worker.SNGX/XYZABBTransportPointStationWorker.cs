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
    public class XYZABBTransportPointStationWorker : OrderWorkerAbstract<XYZABBWorkerBusiness>
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
        public  OperateResult RegisterHandler()
        {
            //foreach (AssistantDevice assistant in WorkerAssistants)
            //{
            //    if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.TransportDevice))
            //    {
            //        //AgvDeviceAbstract agvDevice = assistant.Device as AgvDeviceAbstract;
            //        //if (agvDevice != null)
            //        //{
            //        //    WorkerBusiness.NotifyExeResultEvent += NotifyExeResultEventHandler;
            //        //}
            //    }
            //}

            return OperateResult.CreateSuccessResult();
        }

      

        //public ResponseResult NotifyExeResultEventHandler(int wcsAgvId, int exeStatus, string taskNo)
        //{
        //    AssistantDevice assistant = GetAssistantById(wcsAgvId);
        //    if (assistant == null)
        //    {
        //        string msg = string.Format("根据设备编号:{0} 找不到对应的设备", wcsAgvId);
        //        LogMessage(msg, EnumLogLevel.Error, true);
        //        return ResponseResult.CreateFailedResult(msg);
        //    }
        //    AgvDeviceAbstract agvDevice = assistant.Device as AgvDeviceAbstract;
        //    if (agvDevice == null)
        //    {
        //        string msg = string.Format("设备：{0} 不是Agv设备，请检查设备编号是否配置正确", assistant.Device.Name + assistant.Device.Id);
        //        LogMessage(msg, EnumLogLevel.Error, true);
        //        return ResponseResult.CreateFailedResult(msg);
        //    }
        //    AgvMoveStepEnum step = (AgvMoveStepEnum)exeStatus;
        //    int orderId = int.Parse(taskNo);
        //   OperateResult opResult= agvDevice.HandleMoveStepChange(orderId, step);
        //    if (opResult.IsSuccess == true)
        //    {
        //        return ResponseResult.CreateSuccessResult();
        //    }
        //    else
        //    {
        //        return ResponseResult.CreateFailedResult(opResult.Message);
        //    }
        //}


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
            OrderWorkerViewModel<XYZABBWorkerBusiness> viewModel = new OrderWorkerViewModel<XYZABBWorkerBusiness>(this);
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
