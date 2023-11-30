using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Load.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public  class LoadPlace : DeviceBaseAbstract
    {
        public override void RefreshDeviceState()
        {
            return;
        }

        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected DeviceBusinessBaseAbstract deviceBusiness;
        protected DeviceControlBaseAbstract deviceControl;
        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.deviceBusiness = business;
            this.deviceControl = control;
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            return  OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetDeviceRealData()
        {
        return     OperateResult.CreateSuccessResult();
        }

        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            if (this.CurAddress.IsContain(destAddr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return deviceBusiness.ComputeNextAddr(destAddr);
        }

     
        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            LoadPlaceViewModel viewModel = new LoadPlaceViewModel(this);
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
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
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
