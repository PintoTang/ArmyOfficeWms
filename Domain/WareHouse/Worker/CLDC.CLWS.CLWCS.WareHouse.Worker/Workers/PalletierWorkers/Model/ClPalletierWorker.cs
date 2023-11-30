using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class ClPalletierWorker : PalletierWorkerAbstract<PalletierWorkerBusinessAbstract>
    {

        public override OperateResult PalletierFinishHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content)
        {
            OperateResult finishResult = WorkerBusiness.PalletierFinishHandle(device, count, content);
            if (finishResult.IsSuccess)
            {
                LogMessage(finishResult.Message, EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(finishResult.Message, EnumLogLevel.Error, true);
            }
            return finishResult;
        }

        public override OperateResult PalletizerFinishEachHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content)
        {
            OperateResult finishResult = WorkerBusiness.PalletizerFinishEachHandle(device, count, content);
            if (finishResult.IsSuccess)
            {
                LogMessage(finishResult.Message, EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(finishResult.Message, EnumLogLevel.Error, true);
            }
            return finishResult;
        }

        public override OperateResult NotifyOutbound(DeviceBaseAbstract device)
        {
            OperateResult outBound = WorkerBusiness.NotifyOutbound(device);
            if (outBound.IsSuccess)
            {
                LogMessage(outBound.Message, EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(outBound.Message, EnumLogLevel.Error, true);
            }
            return outBound;
        }


        public override OperateResult GetWorkerRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetWorkerRealData()
        {
            return OperateResult.CreateSuccessResult();
        }

      

        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            PalletierWorkerDeviceViewModel viewModel = new PalletierWorkerDeviceViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;

        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<ClPalletierWorker, PalletierWorkerProperty> viewModel = new WorkerConfigViewModel<ClPalletierWorker, PalletierWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected internal override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = WorkerProperty.Name;
                this.WorkSize = WorkerProperty.WorkSize;
                this.WorkerName = new DeviceName(WorkerProperty.WorkerName);
                this.NameSpace = WorkerProperty.NameSpace;
                this.ClassName = WorkerProperty.ClassName;
                this.Id = WorkerProperty.WorkerId;

                this.WorkerBusiness.Name = WorkerProperty.BusinessHandle.Name;
                this.WorkerBusiness.ClassName = WorkerProperty.BusinessHandle.ClassName;
                this.WorkerBusiness.NameSpace = WorkerProperty.BusinessHandle.NameSpace;

                OperateResult initWorkerConfig = this.InitConfig();
                if (!initWorkerConfig.IsSuccess)
                {
                    return initWorkerConfig;
                }

                OperateResult initBusinessConfig = this.WorkerBusiness.InitConfig();
                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
                result.IsSuccess = false;
            }
            return result;
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
            PalletierWorkerDetailView view = new PalletierWorkerDetailView();
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
