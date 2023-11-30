using System;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.ABB.XYZABB.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.ABB.XYZABB.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// XYZ ABB机器人
    /// </summary>
    public class XYZABB : AgvDeviceAbstract
    {
        public override OperateResult RegisterOrderFinishFlag(TransportMessage transMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override void HandleOrderValueChange(int value)
        {
            return;
        }

        public override OperateResult IsCanChangeControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            return false;
        }

        public override OperateResult HandleMoveStepChange(int orderId, AgvMoveStepEnum moveStep)
        {
            string msg = string.Format("指令：{0} 当前执行步骤：{1}", orderId, moveStep.GetDescription());
            LogMessage(msg, EnumLogLevel.Info, true);

            if (moveStep.Equals(AgvMoveStepEnum.Finish))
            {
                HandleDeviceChange(this.DeviceName, DeviceChangeModeEnum.OrderFinish, orderId);
            }
            CurMoveStep = moveStep;
            return OperateResult.CreateSuccessResult();
        }



        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            XYZABBViewModel viewModel = new XYZABBViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            DeviceShowCard view = new DeviceShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            return configView;
        }

        protected override Window CreateAssistantView()
        {
            UCXYZABBView assistantView = new UCXYZABBView();
            return assistantView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            XYZABBDetailView view = new XYZABBDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            throw new NotImplementedException();
        }
    }
}
