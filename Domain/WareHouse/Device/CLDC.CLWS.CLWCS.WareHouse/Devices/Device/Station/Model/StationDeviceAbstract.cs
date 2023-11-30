using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Station.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.Model
{
    /// <summary>
    /// 科陆控制的可出可入站台设备
    /// </summary>
    public abstract class StationDeviceAbstract : DeviceBaseAbstract, IChangeMode
    {

        private StationProperty _deviceProperty = new StationProperty();

        public StationProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }


      
        #region 站台的状态属性
        /// <summary>
        /// 当前的方向信息
        /// </summary>
        public TransportDirectionEnum CurDirection
        {
            get { return _curDirection; }
            set
            {
                _curDirection = value;
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
        /// 是否已放行
        /// </summary>
        public bool HasReleased
        {
            get { return _hasReleased; }
            set
            {
                _hasReleased = value;
                RaisePropertyChanged();
            }
        }

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


        #endregion

        protected ITransportManage TransportManageHandler { get; set; }

        /// <summary>
        /// 未完成指令的数量
        /// </summary>
        public int UnFinishedTaskCount
        {
            get
            {
                return UnFinishedTask.Lenght;
            }
        }

        private UniqueDataObservablePool<TransportMessage> _unFinishedTask = new UniqueDataObservablePool<TransportMessage>();

        public UniqueDataObservablePool<TransportMessage> UnFinishedTask
        {
            get { return _unFinishedTask; }
            set { _unFinishedTask = value; }
        }

        public void AddUnfinishedTask(TransportMessage transport)
        {
            OperateResult result = UnFinishedTask.AddPool(transport);
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("搬运信息：{0} 添加到待完成搬运信息失败", transport.TransportOrderId), EnumLogLevel.Error, true);
            }
            transport.OwnerId = Id;

            LogMessage(string.Format("成功接收搬运信息：{0} 等待设备搬运完成", transport.TransportOrderId), EnumLogLevel.Info, true);

        }

        public void RemoveUnfinishedTask(TransportMessage transport)
        {
            OperateResult result = UnFinishedTask.RemovePool(o => o.UniqueCode.Equals(transport.UniqueCode));
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("搬运信息：{0} 从未完成搬运信息移除失败", transport.TransportOrderId), EnumLogLevel.Error, true);
            }
        }

        public void RemoveUnfinishedTask(int taskId)
        {
            OperateResult result = UnFinishedTask.RemovePool(o => o.TransportOrderId.Equals(taskId));
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("搬运信息：{0} 从未完成搬运信息移除失败", taskId), EnumLogLevel.Error, true);
            }
        }


        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }

        public override void RefreshDeviceState()
        {
            IsHasTask = CurOrderValue > 0;
        }


        private int _curOrderValue = 0;

        public int CurOrderValue
        {
            get
            {
                return _curOrderValue;
            }
            set
            {
                if (_curOrderValue != value)
                {
                    _curOrderValue = value;
                }
                RaisePropertyChanged("CurOrderValue");
            }
        }

        private int _curDestValue = 0;

        public int CurDestValue
        {
            get
            {
                return _curDestValue;
            }
            set
            {
                if (_curDestValue != value)
                {
                    _curDestValue = value;
                }
                RaisePropertyChanged("CurDestValue");
            }
        }

        public virtual void HandleOrderValueChange(int newValue)
        {
            bool isNeedHandle = DeviceBusiness.IsNeedHandleOrderValue(newValue);
            if (isNeedHandle)
            {
                string msg = string.Format("监控到指令编号值变化：从{0}--->{1}的变化", CurOrderValue, newValue);
                if (newValue > 0)
                {
                    RemoveUnfinishedTask(newValue);
                }
                LogMessage(msg, EnumLogLevel.Info, false);
                if (HandleOrderValueChangeEvent != null)
                {
                    HandleOrderValueChangeEvent(DeviceName, newValue);
                }
            }
            CurOrderValue = newValue;
            CurDirection = TransportDirectionEnum.Forward;
        }


        private void HandleDestValueChange(int newValue)
        {
            bool isNeedHandle = DeviceBusiness.IsNeedHandleDestValue(newValue);
            if (isNeedHandle)
            {
                string msg = string.Format("监控到方向值变化：从{0}--->{1}的变化", CurDestValue, newValue);
                LogMessage(msg, EnumLogLevel.Info, false);
            }
            CurDestValue = newValue;
        }

        public override OperateResult ParticularStart()
        {
            OperateResult registerResult = RegisterValueMonitor();
            return registerResult;
        }

        private OperateResult RegisterValueMonitor()
        {
            OperateResult registerOrderChange = RegisterValueChange(DataBlockNameEnum.OPCOrderIdDataBlock, HandleOrderValueChange);
            if (!registerOrderChange.IsSuccess)
            {
                return registerOrderChange;
            }
            OperateResult registerDestChange = RegisterValueChange(DataBlockNameEnum.DestinationDataBlock,
                HandleDestValueChange);
            if (!registerDestChange.IsSuccess)
            {
                return registerDestChange;
            }

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

        private void IsLoadedValueChange(bool isLoadedValueChange)
        {
            IsLoaded = isLoadedValueChange;
        }

        private void TranslationRvValueChange(bool translationRvValue)
        {
            IsTranslationRv = translationRvValue;
        }

        private void TranslationFwValueChange(bool translationFwValue)
        {
            IsTranslationFw = translationFwValue;
        }

        private void AllFaultChange(bool faultValue)
        {
            IsHasError = faultValue;
            if (faultValue)
            {
                string faultDesc = GetFaultMessage();
                HandleFault(faultDesc);
            }
        }

        protected virtual void HandleFault(string faultDesc)
        {
        }

        private string GetFaultMessage()
        {
            List<DataBlockNameEnum> faultDataBlocks = new List<DataBlockNameEnum> 
            {
                DataBlockNameEnum.IsTranslationDriverFault,
                DataBlockNameEnum.IsTranslationTimeOut,
                DataBlockNameEnum.IsTranslationSwitchFault,
                DataBlockNameEnum.IsCrashStop,
                DataBlockNameEnum.IsTranslationSensorFault,
                DataBlockNameEnum.IsUpDownFault,
                DataBlockNameEnum.IsUpDownDriverFault,
                DataBlockNameEnum.IsUpDownTimeOut,
                DataBlockNameEnum.IsUpDownSwitchFault,
            };
            List<bool> list=this.DeviceControl.Communicate.ReadBoolByBlockEnums(this.Id, faultDataBlocks);
            StringBuilder sb = new StringBuilder("");
            if(list==null|| list.Count == 0)
            {
                return "";
            }
            for(int i=0;i<list.Count;i++)
            {
                if (list[i])
                {
                    if (!string.IsNullOrEmpty(sb.ToString()))
                    {
                        sb.Append("；");
                    }
                    sb.Append(faultDataBlocks[i].GetDescription());
                }
            }
            return sb.ToString();
        }

        public DeviceDelegate.HandleOrderValueChange HandleOrderValueChangeEvent;

        private OperateResult CheckCanChangeMode(DeviceModeEnum destMode)
        {
            OperateResult businessCheck = DeviceBusiness.IsCanChangeMode(destMode);
            if (businessCheck.IsSuccess)
            {
                OperateResult controlCheck = DeviceControl.IsCanChangeMode(destMode);
                if (controlCheck.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult();
                }
                else
                {
                    return controlCheck;
                }
            }
            else
            {
                return businessCheck;
            }

        }

        public StationDeviceBusinessAbstract DeviceBusiness { get; set; }
        public StationDeviceControlAbstract DeviceControl { get; set; }

        /// <summary>
        /// 注册OPC监控的值变化处理
        /// </summary>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockEnum, monitervaluechange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;
        }

        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockNameEnum,
            CallbackContainOpcBoolValue monitorValueChange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockNameEnum, monitorValueChange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;

        }

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as StationDeviceBusinessAbstract;
            this.DeviceControl = control as StationDeviceControlAbstract;
            TransportManageHandler = DependencyHelper.GetService<ITransportManage>();
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "StationDeviceBusinessAbstract", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "StationDeviceControlAbstract", control.GetType().FullName);

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

                string stationPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        DeviceProperty = (StationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(StationProperty));
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

                string initMode = DeviceProperty.Config.DeviceMode.Trim();
                
                _staticDeviceMode = (DeviceModeEnum)Enum.Parse(typeof(DeviceModeEnum), initMode);

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private DeviceModeEnum _curMode = DeviceModeEnum.Default;

        public DeviceModeEnum CurMode
        {
            get
            {
                return _curMode;
            }
            set
            {
                this.DeviceBusiness.CurMode = value;
                this.DeviceControl.CurMode = value;
                _curMode = value;
            }
        }

        /// <summary>
        /// 站台模式
        /// </summary>
        public DeviceModeEnum StaticDeviceMode
        {
            get { return _staticDeviceMode; }
            set { _staticDeviceMode = value; }
        }

        private DeviceModeEnum _staticDeviceMode = DeviceModeEnum.Default;
        private TransportDirectionEnum _curDirection = TransportDirectionEnum.None;
        private bool _isLoaded;
        private bool _hasReleased;
        private bool _isTranslationFw;
        private bool _isTranslationRv;

        public abstract OperateResult IsCanChangeMode(DeviceModeEnum destMode);

        /// <summary>
        /// 切换出入库模式
        /// </summary>
        public OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            OperateResult checkResult = CheckCanChangeMode(destMode);
            if (checkResult.IsSuccess)
            {
                OperateResult controlChange = DeviceControl.ChangeMode(destMode);
                if (controlChange.IsSuccess)
                {
                    OperateResult businessChange = DeviceBusiness.ChangeMode(destMode);
                    if (businessChange.IsSuccess)
                    {
                        LogMessage(string.Format("模式已成功切换为：{0}", destMode), EnumLogLevel.Info, true);
                        return OperateResult.CreateSuccessResult();
                    }
                    else
                    {
                        LogMessage(string.Format("模式切换失败：{0} 原因：{1}", destMode, businessChange.Message), EnumLogLevel.Info, true);
                        return businessChange;
                    }
                }
                else
                {
                    LogMessage(string.Format("模式切换失败：{0} 原因：{1}", destMode, controlChange.Message), EnumLogLevel.Info, true);
                    return controlChange;
                }
            }
            else
            {
                return OperateResult.CreateFailedResult(string.Format("当前模式：{0} 此时不允许切换到指定模式：{1}  原因：{2}", CurMode, destMode, checkResult.Message), 1);
            }
        }

        /// <summary>
        /// 获取货物的属性
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult<SizeProperties> GetGoodsProperties();

        /// <summary>
        /// 检查当前的模式是不是指定的模式
        /// </summary>
        /// <param name="destMode"></param>
        /// <returns></returns>
        public bool CheckMode(DeviceModeEnum destMode)
        {
            bool controlCheck = DeviceControl.CheckMode(destMode);
            if (controlCheck)
            {
                bool businessCheck = DeviceBusiness.CheckMode(destMode);
                if (businessCheck)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }

}
