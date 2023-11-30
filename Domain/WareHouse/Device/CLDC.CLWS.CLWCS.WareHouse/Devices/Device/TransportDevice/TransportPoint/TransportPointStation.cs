using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public sealed class TransportPointStation : TransportDeviceBaseAbstract, IReadyMonitor
    {

        private bool _isTranslationFw;
        private bool _isTranslationRv;
        private bool _isLoaded;

        /// <summary>
        /// 是否正转
        /// </summary>
        public bool IsTranslationFw
        {
            get { return _isTranslationFw; }
            set
            {
                _isTranslationFw = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否载货
        /// </summary>
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否反转
        /// </summary>
        public bool IsTranslationRv
        {
            get { return _isTranslationRv; }
            set
            {
                _isTranslationRv = value;
                RaisePropertyChanged();
            }
        }


        private RollerProperty _deviceProperty = new RollerProperty();

        public RollerProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }

        public override OperateResult DoTransportJob(TransportMessage transport)
        {
            OperateResult businessResult = DeviceBusiness.DoJob(transport);
            if (!businessResult.IsSuccess)
            {
                string msg = string.Format("指令：{0} 业务处理发生错误：{1}", transport.TransportOrderId, businessResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }

            OperateResult controlResult = DeviceControl.DoJob(transport);
            if (!controlResult.IsSuccess)
            {
                string msg = string.Format("指令：{0} 业务控制发生错误：{1}", transport.TransportOrderId, controlResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }

            LogMessage(string.Format("开始创建搬运信息：{0} 判断超时", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.CheckTimeOut.Check(TransportTimeOutAction);
            LogMessage(string.Format("创建搬运信息：{0} 判断超时完成", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.UpdateDateTime = DateTime.Now;
            transport.TransportStatus = TransportResultEnum.UnFinish;
            CurWorkState = WorkStateMode.Working;

            LogMessage(string.Format("开始更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);
            TransportManageHandler.UpdateTransportAsync(transport);
            LogMessage(string.Format("结束更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);

            CurWorkState = WorkStateMode.Working;

            LogMessage(string.Format("指令：{0} 条码：{1} 已成功放行到下一个设备：{2} ", transport.TransportOrderId, transport.PileNo, transport.DestDevice.Name), EnumLogLevel.Info, true);
            return controlResult;
        }

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            OperateResult baseInitResult = base.ParticularInitlize(business, control);
            if (!baseInitResult.IsSuccess)
            {
                return baseInitResult;
            }
            OperateResult registerResult = RegisterReadyValueChange(DataBlockNameEnum.OPCOrderIdDataBlock, HandleOrderValueChange);
            if (!registerResult.IsSuccess)
            {
                return registerResult;
            }
            return OperateResult.CreateSuccessResult();
        }


        public override bool Accessible(Addr destAddr)
        {
            return CurAddress.IsContain(destAddr);
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
                        DeviceProperty = (RollerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(RollerProperty));
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

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateFailedResult();
        }

        private void AllFaultChange(bool faultValue)
        {
            IsHasError = faultValue;
        }

        private void TranslationRvValueChange(bool translationRvValue)
        {
            IsTranslationRv = translationRvValue;
        }

        private void TranslationFwValueChange(bool translationFwValue)
        {
            IsTranslationFw = translationFwValue;
        }

        private void IsLoadedValueChange(bool isLoadedValueChange)
        {
            IsLoaded = isLoadedValueChange;
        }

        private OperateResult RegisterValueMonitor()
        {
            
            OperateResult registerAllFault = RegisterValueChange(DataBlockNameEnum.IsTranslationAllFault, AllFaultChange);
            if (!registerAllFault.IsSuccess)
            {
                return registerAllFault;
            }

            OperateResult registerTranslationFw = RegisterValueChange(DataBlockNameEnum.IsTranslationFw,
                TranslationFwValueChange);
            if (!registerTranslationFw.IsSuccess)
            {
                return registerTranslationFw;
            }

            OperateResult registerTranslationRv = RegisterValueChange(DataBlockNameEnum.IsTranslationRv,
                TranslationRvValueChange);
            if (!registerTranslationRv.IsSuccess)
            {
                return registerTranslationRv;
            }

            OperateResult registerIsLoaded = RegisterValueChange(DataBlockNameEnum.IsLoaded,
               IsLoadedValueChange);
            if (!registerIsLoaded.IsSuccess)
            {
                return registerIsLoaded;
            }

            return registerIsLoaded;
        }
        public override OperateResult ParticularStart()
        {
           OperateResult registerResult= RegisterValueMonitor();
           if (!registerResult.IsSuccess)
           {
               return registerResult;
           }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeControlState(ControlStateMode destState)
        {
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult RegisterOrderFinishFlag(TransportMessage transMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override void HandleOrderValueChange(int value)
        {
            bool isNeedHandle = IsNeedHandleReadyValue(value);
            if (isNeedHandle)
            {
                if (ReadyValueChangeEvent != null)
                {
                    ReadyValueChangeEvent(DeviceName, value);
                }
            }
            if (DeviceBusiness.IsNeedHanldeOrderValue(value))
            {
                OperateResult handleResult = HandleDeviceChange(DeviceName, DeviceChangeModeEnum.OrderFinish, value);
            }
        }



        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<TransportPointStation, RollerProperty> viewModel = new DeviceConfigViewModel<TransportPointStation, RollerProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            RollerDeviceViewModel viewModel = new RollerDeviceViewModel(this);
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

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            ClRollerDeviceDetailView view = new ClRollerDeviceDetailView();
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

        public OperateResult RegisterReadyValueChange(DataBlockNameEnum dbNameEnum, CallbackContainOpcValue valueChangeCallBack)
        {
            return this.DeviceControl.RegisterValueChange(dbNameEnum, valueChangeCallBack);
        }

        public ReadyValueChangeDelegate ReadyValueChangeEvent { get; set; }
        public bool IsNeedHandleReadyValue(int value)
        {
            if (value.Equals(-1000))
            {
                return true;
            }
            return false;
        }
    }
}
