using System;
using System.IO;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 罗伯泰克堆垛机
    /// </summary>
    public sealed class RobotTecStackingcrane : StackingcraneDeviceAbstract
    {

        private RobotTecStackingcraneProperty _deviceProperty = new RobotTecStackingcraneProperty();

        public RobotTecStackingcraneProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }

        public override bool Accessible(Addr destAddr)
        {
            return false;
        }

        public override OperateResult ClearFaultCode()
        {
            return DeviceControl.ClearFaultCode();
        }

        public override void HandleInProcessOrderValueChange()
        {
            if (CurTransport == null)
            {
                return;
            }
            HandleDeviceChange(DeviceName, DeviceChangeModeEnum.OrderFinish, CurTransport.TransportOrderId);
        }

        public override void HandleOrderValueChange(int value)
        {
            bool isNeedHandle = DeviceBusiness.IsNeedHanldeOrderValue(value);
            if (!isNeedHandle)
            {
                return;
            }
            else
            {
                return;
            }

            ///此处根据指令的处理情况再处理别的业务
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            if (!CurConnectState.Equals(ConnectState.Online))
            {
                string msg = string.Format("当前设备连接状态为：{0} 不可切换调度状态：{1}", CurConnectState, destState);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (!CurControlMode.Equals(ControlStateMode.Auto))
            {
                string msg = string.Format("当前设备控制状态为：{0} 不可切换调度状态：{1}", CurControlMode, destState);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult RegisterOrderFinishFlag(TransportMessage transMsg)
        {
            LogMessage(string.Format("开始注册指令完成事件： 注册的变化由{0}--->{1}", transMsg.TransportOrderId, 0), EnumLogLevel.Debug, false);
            OperateResult registerResult = DeviceControl.RegisterFromStartToEndStatus(DataBlockNameEnum.InProgressOrder, transMsg.TransportOrderId, 0, HandleInProcessOrderValueChange);
            if (!registerResult.IsSuccess)
            {
                LogMessage(string.Format("注册指令完成失败：{0} 注册的变化由{1}--->{2}", registerResult.Message, transMsg.TransportOrderId, 0), EnumLogLevel.Error, false);
            }
            LogMessage(string.Format("注册指令完成成功 注册的变化由{0}--->{1}", transMsg.TransportOrderId, 0), EnumLogLevel.Debug, false);
            return OperateResult.CreateSuccessResult();
        }

        protected override void HandleDeviceCurColumnChange(int newValue)
        {
            this.CurColumn = newValue;
        }

        protected override void HandleDeviceWorkStatusChange(int newValue)
        {
            return;
        }

        public override OperateResult IsCanChangeControlState(ControlStateMode destState)
        {
            if (!CurConnectState.Equals(ConnectState.Online))
            {
                string msg = string.Format("当前设备连接状态为：{0} 不可切换控制状态：{1}", CurConnectState, destState);
                return OperateResult.CreateFailedResult(msg, 1);
            }
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
                        DeviceProperty = (RobotTecStackingcraneProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(RobotTecStackingcraneProperty));
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

        protected override OperateResult RegisterValueChange()
        {
            //1.注册正在处理的命令值变化
            //2.注册异常代码的值变化
            //3.注册设备行走状态的变化
            //4.注册设备运行状态的变化

            OperateResult registerCurColumn = DeviceControl.RegisterValueChange(DataBlockNameEnum.DeviceCurColumn, HandleDeviceCurColumnChange);
            if (!registerCurColumn.IsSuccess)
            {
                return registerCurColumn;
            }
            OperateResult registerWorkStatus = DeviceControl.RegisterValueChange(DataBlockNameEnum.DeviceWorkStatus, HandleDeviceWorkStatusChange);
            if (!registerWorkStatus.IsSuccess)
            {
                return registerWorkStatus;
            }

            DeviceControl.NotifyDeviceExceptionEvent += DeviceExceptionHandle;


            return OperateResult.CreateSuccessResult();
        }



        public override OperateResult HandleRestoreData()
        {
            OperateResult baseHandleResult = base.HandleRestoreData();
            if (!baseHandleResult.IsSuccess)
            {
                return baseHandleResult;
            }
            //1.当设备存在未完成指令的时候读取已完成的命令编号
            //2.获取待完成的指令中是否有该命令编号的未完成指令，如果有上报该指令完成，如果没有则不处理
            if (UnFinishedTaskCount > 0)
            {
                OperateResult<int> finishedOrder = DeviceControl.GetFinishedOrder();
                if (!finishedOrder.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult();
                }
                int orderValue = finishedOrder.Content;
                OperateResult result = HandleDeviceChange(DeviceName, DeviceChangeModeEnum.OrderFinish, orderValue);
                if (!result.IsSuccess)
                {
                    if (CurTransport != null)
                    {
                        DeviceControl.RegisterFromStartToEndStatus(DataBlockNameEnum.InProgressOrder, CurTransport.TransportOrderId, 0, HandleInProcessOrderValueChange);
                    }
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            RobotTcViewModel viewModel = new RobotTcViewModel(this);
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
            DeviceConfigViewModel<RobotTecStackingcrane, RobotTecStackingcraneProperty> viewModel = new DeviceConfigViewModel<RobotTecStackingcrane, RobotTecStackingcraneProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
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
    }
}
