using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.ClouRcs.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.ClouRcs.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.ClouRcs.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.DataValidation;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    [ClouDesc("黄云龙","科陆自主研发的RCS系统对接设备","四川能投")]
    public class ClouAgvRcs : AgvDeviceAbstract
    {
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
        private readonly Dictionary<int, TaskExcuteStatusType> _faultCodeDictionary = new Dictionary<int, TaskExcuteStatusType>();

        private void InitilizeFaultCodeDictionary()
        {
            _faultCodeDictionary.Add(2, TaskExcuteStatusType.OutButEmpty);
            _faultCodeDictionary.Add(1, TaskExcuteStatusType.InButExist);
        }
        public override OperateResult ParticularConfig()
        {
            InitilizeFaultCodeDictionary();

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
                        DeviceProperty = (ClouAgvRcsProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(ClouAgvRcsProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：ClouAgvRcsProperty", devicePropertyXml));
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
            ClouAgvRcsViewModel viewModel = new ClouAgvRcsViewModel(this);
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
            ClouAgvRcsAssistantView assistantView = new ClouAgvRcsAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<ClouAgvRcs, ClouAgvRcsProperty> viewModel = new DeviceConfigViewModel<ClouAgvRcs, ClouAgvRcsProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }
        private ClouAgvRcsProperty _deviceProperty = new ClouAgvRcsProperty();

        public ClouAgvRcsProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }
        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            ClouAgvRcsDetailView view = new ClouAgvRcsDetailView();
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

        //public override OperateResult HandleDeviceChange(DeviceName deviceName, DeviceChangeModeEnum changeMode, int value)
        //{
        //    return base.HandleDeviceChange(deviceName, changeMode, value);
        //}
    }
}
