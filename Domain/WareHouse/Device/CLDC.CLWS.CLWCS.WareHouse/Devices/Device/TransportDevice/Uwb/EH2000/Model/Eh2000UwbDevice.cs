using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public sealed class Eh2000UwbDevice : TransportDeviceBaseAbstract
    {

        private double _x = 1000.0;
        private double _y = 800.0;
        private double _z = 0.0;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                RaisePropertyChanged();
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                RaisePropertyChanged();
            }
        }

        public double Z
        {
            get { return _z; }
            set
            {
                _z = value;
                RaisePropertyChanged();
            }
        }


        public override OperateResult Availabe()
        {
            OperateResult baseAvailable = base.Availabe();
            if (!baseAvailable.IsSuccess)
            {
                return baseAvailable;
            }
            if (StandByStatus.Equals(UwbDeviceStandByEnum.StandByOff))
            {
                return OperateResult.CreateFailedResult("处于非待命状态");
            }
            return OperateResult.CreateSuccessResult();
        }

        public UwbDeviceStandByEnum StandByStatus
        {
            get { return _standByStatus; }
            set { _standByStatus = value; }
        }

        public string WorkArea { get; set; }

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            OperateResult baseInitResult = base.ParticularInitlize(business, control);
            if (!baseInitResult.IsSuccess)
            {
                return baseInitResult;
            }
            DeviceBusiness = business as Eh2000UwbBusiness;
            DeviceControl = control as Eh2000UwbControl;
            if (DeviceBusiness == null)
            {
                return OperateResult.CreateFailedResult("转换为恒高Uwb业务处理失败");
            }
            if (DeviceControl == null)
            {
                return OperateResult.CreateFailedResult("转换为恒高Uwb控制失败");
            }
            _architecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult ParticularStart()
        {
            #region 获取身份信息


            #endregion
            return OperateResult.CreateSuccessResult();
        }
        private Eh2000UwbProperty _deviceProperty = new Eh2000UwbProperty();

        private UwbDeviceStandByEnum _standByStatus = UwbDeviceStandByEnum.StandByOn;

        public Eh2000UwbProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
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

                string stationPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        DeviceProperty = (Eh2000UwbProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(Eh2000UwbProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：StationProperty", stationPropertyXml));
                }

                this.ExposedUnId = DeviceProperty.Config.ExposedUnId;

                this.WorkArea = DeviceProperty.Config.WorkArea;

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
            return CurAddress.IsContain(destAddr);
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            Eh2000UwbDeviceViewModel viewModel = new Eh2000UwbDeviceViewModel(this);
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
            Eh2000AssistantView assistantView = new Eh2000AssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<Eh2000UwbDevice, Eh2000UwbProperty> viewModel = new DeviceConfigViewModel<Eh2000UwbDevice, Eh2000UwbProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            Eh2000DetailView view = new Eh2000DetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        /// <summary>
        /// 当前执行的指令
        /// </summary>
        public TransportMessage CurTransport
        {
            get
            {
                return UnFinishedTask.DataPool.FirstOrDefault(t => t.TransportStatus.Equals(TransportResultEnum.UnFinish));
            }
        }

        public new Eh2000UwbBusiness DeviceBusiness { get; set; }

        public new Eh2000UwbControl DeviceControl { get; set; }


        private void SendMsgToUwbCard(PublishEinkCmd pulishEinkMsg)
        {
            WebApiInvokeCmd cmd = new WebApiInvokeCmd(this.DeviceControl.Http, Eh2000UwbApiEnum.publishEink.ToString(), pulishEinkMsg.ToString());
            DeviceControl.EinkApi(cmd);
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
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
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

        public void HandleRealPosition(double x, double y, double z)
        {
            this.X = x * 1000;
            this.Y = y * 1000;
            this.Z = z * 1000;
        }

        public void HandleInOrOutArea(string areaId, InOrOutEnum action)
        {
            //判断标签是否进入到工作区域
            if (string.IsNullOrEmpty(WorkArea))
            {
                //LogMessage("WorkArea没配置不对设备进行状态控制", EnumLogLevel.Debug, false);
                return;
            }
            if (areaId.Equals(WorkArea))
            {
                if (action.Equals(InOrOutEnum.In))
                {
                    this.StandByStatus = UwbDeviceStandByEnum.StandByOn;
                    LogMessage("开始准备接收任务",EnumLogLevel.Info, true);
                }
                else
                {
                    this.StandByStatus = UwbDeviceStandByEnum.StandByOff;
                    LogMessage("将不接收任务", EnumLogLevel.Info, true);
                }
            }
        }

        private IWmsWcsArchitecture _architecture;
        public void HandleConfirmMessage(string areaId, string areaName)
        {
            if (!string.IsNullOrEmpty(WorkArea))
            {
                if (WorkArea.Equals(areaId))
                {
                    return;    
                }
            }
            if (CurTransport == null)
            {
                SendMsgToUwbCard(new PublishEinkCmd(this.ExposedUnId, "当前不存在任务信息，请核实", EinkNotifyTypeEnum.Alarm, EinkVoiceTypeEnum.NoVoice, EinkShakeTypeEnum.HasShake));
                LogMessage(string.Format("卡号：{0} 在区域：{1} 上报确认任务，当前不存在任务信息，请核实",areaId,areaName),EnumLogLevel.Error, true);
                return;
            }

            OperateResult<string> getShowNameResult = _architecture.WcsToShowName(CurTransport.DestAddr.FullName);
            if (!getShowNameResult.IsSuccess)
            {
                LogMessage(string.Format("根据Wcs地址：{0} 获取区域名失败", CurTransport.DestAddr), EnumLogLevel.Error, true);
                SendMsgToUwbCard(new PublishEinkCmd(this.ExposedUnId, string.Format("区域:{0} Wcs系统未配置",areaName), EinkNotifyTypeEnum.Alarm, EinkVoiceTypeEnum.NoVoice, EinkShakeTypeEnum.HasShake));
                return;
            }

            if (!getShowNameResult.Content.Equals(areaName))
            {
                string logMsg = string.Format("期望地址：{0} 确认区域名称：{1} 期望地址与确认区域不相符，请核实", getShowNameResult.Content,areaName);
                LogMessage(logMsg, EnumLogLevel.Error, true);
                SendMsgToUwbCard(new PublishEinkCmd(this.ExposedUnId, string.Format("期望地址：{0},确认地址：{1}，请核实", getShowNameResult.Content,areaName), EinkNotifyTypeEnum.Alarm, EinkVoiceTypeEnum.NoVoice, EinkShakeTypeEnum.HasShake));
                return;
            }

            OperateResult finishTransportResult = FinishTransport(CurTransport, FinishType.AutoFinish);
            if (!finishTransportResult.IsSuccess)
            {
                SendMsgToUwbCard(new PublishEinkCmd(this.ExposedUnId, string.Format("在区域:{0}完成任务失败,请核实", areaName), EinkNotifyTypeEnum.Alarm, EinkVoiceTypeEnum.NoVoice, EinkShakeTypeEnum.HasShake));
                return;
            }
            SendMsgToUwbCard(new PublishEinkCmd(this.ExposedUnId, "任务已完成", EinkNotifyTypeEnum.Alarm, EinkVoiceTypeEnum.NoVoice, EinkShakeTypeEnum.HasShake));
        }

    }
}
