using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClPalletizerDevice : PalletizerDeviceAbstract
    {
        protected override void HandleNeedSignChange(int newValue)
        {
            CurNeedSignValue = newValue==1;
            bool isNeedHandle = DeviceBusiness.IsNeedHandleNeedSignValue(CurNeedSignValue);
            if (!isNeedHandle) return;

            string msg = string.Format("监控到指令编号值变化：从{0}--->{1}的变化", CurOrderValue, newValue);
            LogMessage(msg, EnumLogLevel.Info, false);
            if (NotifyPalletizerOutboundEvent != null)
            {
                 NotifyPalletizerOutboundEvent(this);
            }
            else
            {
                string notifyOutbound = string.Format("碟盘完成事件：NotifyPalletizerOutboundEvent 尚未注册");
                LogMessage(notifyOutbound, EnumLogLevel.Error, true);
            }
        }



        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            return CurAddress.IsContain(destAddr);
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            ClearContent();
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            PalletizerDeviceViewModel viewModel = new PalletizerDeviceViewModel(this);
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

        protected override Window CreateAssistantView()
        {
            return null;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<ClPalletizerDevice, PalletizerDeviceProperty> viewModel = new DeviceConfigViewModel<ClPalletizerDevice, PalletizerDeviceProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new PalletizerDetailView();
            return OperateResult.CreateSuccessResult(view);
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = DeviceProperty.Name;
                this.WorkSize = DeviceProperty.WorkSize;
                this.CurAddress = new Addr(DeviceProperty.CurAddress);
                this.DeviceName = new DeviceName(DeviceProperty.DeviceName);
                this.NameSpace = DeviceProperty.NameSpace;
                this.ClassName = DeviceProperty.ClassName;
                this.Id = DeviceProperty.DeviceId;
                this.IsShowUi = DeviceProperty.IsShowUi;

                this.DeviceControl.Name = DeviceProperty.ControlHandle.Name;
                this.DeviceControl.ClassName = DeviceProperty.ControlHandle.ClassName;
                this.DeviceControl.NameSpace = DeviceProperty.ControlHandle.NameSpace;

                this.DeviceBusiness.Name = DeviceProperty.BusinessHandle.Name;
                this.DeviceBusiness.NameSpace = DeviceProperty.BusinessHandle.NameSpace;
                this.DeviceBusiness.ClassName = DeviceProperty.BusinessHandle.ClassName;

                OperateResult initControlConfig = this.DeviceControl.ParticularInitConfig();
                if (!initControlConfig.IsSuccess)
                {
                    return initControlConfig;
                }

                OperateResult initBusinessConfig = this.DeviceBusiness.ParticularConfig();

                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
        
            }
            return result;
        }
    }
}
