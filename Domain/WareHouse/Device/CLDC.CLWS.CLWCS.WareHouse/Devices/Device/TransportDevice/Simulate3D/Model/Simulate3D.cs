using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Simulate3D.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class Simulate3D : TransportDeviceBaseAbstract
    {

        private TaskOrderDataAbstract _taskOrderDataHandle;

        private FourWayVehicleRcsProperty _deviceProperty = new FourWayVehicleRcsProperty();

        public FourWayVehicleRcsProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }

        public override OperateResult DoTransportJob(TransportMessage transport)
        {
           //不做具体业务
            return OperateResult.CreateSuccessResult();
        }

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
            _taskOrderDataHandle = DependencyHelper.GetService<TaskOrderDataAbstract>();
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                XmlNode xmlNode = doc.GetXmlNode("Device", "DeviceId", Id.ToString());


                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string devicePropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(devicePropertyXml))
                {
                    try
                    {
                        DeviceProperty = (FourWayVehicleRcsProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(FourWayVehicleRcsProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：RobotTecStackingcraneProperty", devicePropertyXml));
                }

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override bool Accessible(Addr destAddr)
        {
            return false;
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            Simulate3DViewModel viewModel = new Simulate3DViewModel(this);
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
            FourWayVehicleRcsAssistantView assistantView = new FourWayVehicleRcsAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<Simulate3D, FourWayVehicleRcsProperty> viewModel = new DeviceConfigViewModel<Simulate3D, FourWayVehicleRcsProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            FourWayVehicleRcsDetailView view = new FourWayVehicleRcsDetailView();
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
            return OperateResult.CreateSuccessResult();
        }


    }
}
