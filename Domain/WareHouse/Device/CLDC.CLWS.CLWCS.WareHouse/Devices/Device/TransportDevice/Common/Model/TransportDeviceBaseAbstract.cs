using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.TransportDevice.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model
{
    /// <summary>
    /// 具有状态数据的设备基类
    /// </summary>
    public abstract class TransportDeviceBaseAbstract : DeviceBaseForTask<TransportMessage>, IStateControl
    {


        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }
        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.OrderDatabaseHandler = DependencyHelper.GetService<OrderDataAbstract>();
            this.DeviceLiveDataHandler = DependencyHelper.GetService<LiveStatusAbstract>();
            TransportManageHandler = DependencyHelper.GetService<ITransportManage>();
            this.DeviceBusiness = business as OrderDeviceBusinessAbstract;
            this.DeviceControl = control as OrderDeviceControlAbstract;
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

        /// <summary>
        /// 指令数据库访问器
        /// </summary>
        protected OrderDataAbstract OrderDatabaseHandler { get; set; }

        protected ITransportManage TransportManageHandler { get; set; }

        public bool Accessible(Addr startAddr, Addr destAddr)
        {
            return DeviceBusiness.Accessible(startAddr, destAddr);
        }

        public override void AddUnfinishedTask(TransportMessage transport)
        {
            OperateResult result = UnFinishedTask.AddPool(transport);
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("搬运信息：{0} 添加到未完成指令失败", transport.TransportOrderId), EnumLogLevel.Error, true);
            }
            transport.OwnerId = Id;
            TransportManageHandler.UpdateTransportAsync(transport);
            LogMessage(string.Format("成功接收搬运信息：{0} 等待设备搬运完成", transport.TransportOrderId), EnumLogLevel.Info, true);
            RefreshDeviceState();
        }

        public override void RemoveUnfinishedTask(TransportMessage transport)
        {
            OperateResult result = UnFinishedTask.RemovePool(o => o.UniqueCode.Equals(transport.UniqueCode));
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("搬运信息：{0} 从未完成搬运信息移除失败", transport.TransportOrderId), EnumLogLevel.Error, true);
            }
            TransportManageHandler.RemoveTransport(transport);
            RefreshDeviceState();

        }


        public OperateResult FinishTransport(TransportMessage transport, FinishType type)
        {
            transport.TransportStatus = TransportResultEnum.Success;
            transport.UpdateDateTime = DateTime.Now;
            transport.CurAddr = transport.DestAddr;
            transport.TransportFinishType = type;
            //应该根据错误的代号判断是否需要释放设备
            this.CurWorkState = WorkStateMode.Free;

            TransportManageHandler.UpdateTransportAsync(transport);

            LogMessage(string.Format("搬运信息：{0} 搬运完成  设备空闲！并已上报完成 完成方式：{1} 对应的指令：{2}", transport.TransportOrderId, type.GetDescription(), transport.TransportOrder), EnumLogLevel.Info, true);
            transport.CheckTimeOut.SetIsSucessful(true);
            RemoveUnfinishedTask(transport);

            //此处打印显示包好变化后指令业务处理成功
            NotifyTaskExcuteStatus(this, transport, TaskExcuteStepStatusEnum.Finished);


            return OperateResult.CreateSuccessResult();
        }

        public virtual OperateResult HandleDeviceChange(DeviceName deviceName, DeviceChangeModeEnum changeMode, int value)
        {

            if (changeMode.Equals(DeviceChangeModeEnum.OrderFinish))
            {

                OperateResult<TransportMessage> finishedTransport = UnFinishedTask.FindData(o => o.TransportOrderId.Equals(value));
                if (!finishedTransport.IsSuccess)
                {
                    string msg = string.Format("不存在未完成指令：{0} 不做处理", value);
                    LogMessage(msg, EnumLogLevel.Info, false);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                return FinishTransport(finishedTransport.Content, FinishType.AutoFinish);

            }
            return OperateResult.CreateSuccessResult();
        }



        /// <summary>
        /// 设备业务处理类
        /// </summary>
        public OrderDeviceBusinessAbstract DeviceBusiness { get; set; }

        /// <summary>
        /// 设备控制处理类
        /// </summary>
        public OrderDeviceControlAbstract DeviceControl { get; set; }



        /// <summary>
        /// 当前执行指令的类型
        /// </summary>
        public OrderTypeEnum CurrentOrderType
        {
            get { return _currentOrderType; }
            set
            {
                if (_currentOrderType != value)
                {
                    _currentOrderType = value;
                    NotifyAttributeChange("CurrentOrderType", value);
                }
            }
        }

        private OrderTypeEnum _currentOrderType = OrderTypeEnum.In;


        private Package _currentPackage;






        /// <summary>
        /// 当前设备的包信息
        /// </summary>
        public Package CurrentPackage
        {
            get { return _currentPackage; }
            set
            {
                _currentPackage = value;
                NotifyAttributeChange("CurrentPackage", value);
            }
        }

        private OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            OperateResult businessCheck = DeviceBusiness.IsCanChangeAbleState(destState);
            if (businessCheck.IsSuccess)
            {
                OperateResult controlCheck = DeviceControl.IsCanChangeAbleState(destState);
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
        private WorkStateMode _curWorkState = WorkStateMode.Free;

        /// <summary>
        /// 工作状态
        /// </summary>
        public WorkStateMode CurWorkState
        {
            get { return _curWorkState; }
            set
            {
                if (_curWorkState != value)
                {
                    _curWorkState = value;
                    RaisePropertyChanged("CurWorkState");
                }
            }
        }

        /// <summary>
        /// 设备处理工作
        /// </summary>
        /// <param name="transport">搬运信息</param>
        /// <returns></returns>
        public virtual OperateResult DoTransportJob(TransportMessage transport)
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
            AddUnfinishedTask(transport);
            LogMessage(string.Format("开始创建搬运信息：{0} 判断超时", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.CheckTimeOut.Check(TransportTimeOutAction);
            LogMessage(string.Format("创建搬运信息：{0} 判断超时完成", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.UpdateDateTime = DateTime.Now;
            transport.TransportStatus = TransportResultEnum.UnFinish;
            CurWorkState = WorkStateMode.Working;

            OperateResult register = RegisterOrderFinishFlag(transport);
            if (!register.IsSuccess)
            {
                LogMessage(string.Format("指令：{0} 下发成功,但是监控指令完成事件失败,原因：{1}", transport.TransportOrderId, register.Message), EnumLogLevel.Error, true);
            }
            LogMessage(string.Format("指令：{0} 条码：{1} 已成功放行到下一个设备：{2} ", transport.TransportOrderId, transport.PileNo, transport.DestDevice.Name), EnumLogLevel.Info, true);

            LogMessage(string.Format("开始更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);
            TransportManageHandler.UpdateTransportAsync(transport);
            LogMessage(string.Format("结束更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);

            return controlResult;
        }

        /// <summary>
        /// 指令执行超时的方法
        /// </summary>
        public virtual void TransportTimeOutAction(string transportGuid)
        {
            OperateResult<TransportMessage> getTransport= UnFinishedTask.FindData(t => t.Guid.Equals(transportGuid));
            if (getTransport.IsSuccess)
            {
                string msg = string.Format("搬运信息：{0} 执行超时，请检查设备是否正常！", getTransport.Content.TransportOrderId);
                LogMessage(msg, EnumLogLevel.Warning, true);
            }
        }

        public abstract OperateResult  RegisterOrderFinishFlag(TransportMessage transMsg);

        /// <summary>
        /// 处理命令编号变化的处理
        /// </summary>
        /// <param name="value"></param>
        public abstract void HandleOrderValueChange(int value);



        //此处应该是监听设备运行参数;设备属性参数;任务执行情况的上报

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

        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockNameEnum,CallbackContainOpcBoolValue monitorValueChange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockNameEnum, monitorValueChange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;

        }


        #region 提供界面操作接口
        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        public OperateResult Start()
        {
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public OperateResult Stop()
        {
            CurRunState = RunStateMode.Stop;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从数据库恢复状态
        /// </summary>
        /// <returns></returns>
        public override OperateResult GetDeviceRealStatus()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<LiveStatusData> deviceData = DeviceLiveDataHandler.GetDeviceLiveDataByDeviceId(this.Id);
                if (deviceData.IsSuccess)
                {
                    LiveStatusData liveData = deviceData.Content;
                    DeviceLiveData = liveData;
                    CurRunState = (RunStateMode)liveData.RunState;
                    CurDispatchState = (DispatchState)liveData.DispatchState;
                    CurControlMode = (ControlStateMode)liveData.ControlState;
                    CurUseState = (UseStateMode)liveData.UseState;
                    result.IsSuccess = true;
                }
                else
                {
                    DeviceLiveData.Alias = Name;
                    DeviceLiveData.ControlState = (int)CurControlMode;
                    DeviceLiveData.DispatchState = (int)CurDispatchState;
                    DeviceLiveData.Id = Id;
                    DeviceLiveData.Name = DeviceName.FullName;
                    DeviceLiveData.RunState = (int)CurRunState;
                    DeviceLiveData.UseState = (int)CurUseState;
                    string msg = string.Format("数据库不存在：{0} {1} 的实时状态记录，将重新插入", Id, Name);
                    LogMessage(msg, EnumLogLevel.Info, false);
                    OperateResult insertResult = DeviceLiveDataHandler.Save(DeviceLiveData);
                    return insertResult;
                }
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        /// <summary>
        /// 从数据库恢复数据
        /// </summary>
        /// <returns></returns>
        public override OperateResult GetDeviceRealData()
        {
            #region 恢复本工作者的未完成搬运信息

            if (TransportManageHandler == null)
            {
                return OperateResult.CreateSuccessResult();
            }
            UnFinishedTask = TransportManageHandler.GetAllUnFinishedTransportByOwnerId(Id);
            if (UnFinishedTask.Count(t => t.TransportStatus.Equals(TransportResultEnum.UnFinish)) > 0)
            {
                IEnumerable<TransportMessage> unFinishedTransportLst = UnFinishedTask.FindAllData(t => t.TransportStatus.Equals(TransportResultEnum.UnFinish));
                foreach (TransportMessage transportMessage in unFinishedTransportLst)
                {
                    OperateResult registerResult = RegisterOrderFinishFlag(transportMessage);
                    if (!registerResult.IsSuccess)
                    {
                        string msg = string.Format("断点恢复，搬运信息：{0} 注册完成信号失败", transportMessage.TransportOrderId);
                        LogMessage(msg, EnumLogLevel.Error, true);
                    }
                }

            }
            #endregion
            RefreshDeviceState();
            return OperateResult.CreateSuccessResult();

        }

        /// <summary>
        /// 更改启用/禁用状态
        /// </summary>
        /// <param name="destState"></param>
        /// <returns></returns>
        public override OperateResult SetAbleState(UseStateMode destState)
        {
            OperateResult check = IsCanChangeAbleState(destState);
            if (check.IsSuccess)
            {
                OperateResult controlEnable = DeviceControl.SetAbleState(destState);
                if (controlEnable.IsSuccess)
                {
                    OperateResult businessEnable = DeviceBusiness.SetAbleState(destState);
                    if (businessEnable.IsSuccess)
                    {
                        CurUseState = destState;
                        DeviceLiveData.UseState = (int)destState;
                        DeviceLiveDataHandler.SaveAsync(DeviceLiveData);
                        return OperateResult.CreateSuccessResult();
                    }
                    else
                    {
                        return businessEnable;
                    }
                }
                else
                {
                    return controlEnable;
                }
            }
            else
            {
                return check;
            }
        }

        public abstract OperateResult IsCanChangeControlState(ControlStateMode destState);

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            OperateResult check = IsCanChangeControlState(destState);
            if (check.IsSuccess)
            {
                OperateResult controlEnable = DeviceControl.SetControlState(destState);
                if (controlEnable.IsSuccess)
                {
                    OperateResult businessEnable = DeviceBusiness.SetControlState(destState);
                    if (businessEnable.IsSuccess)
                    {
                        CurControlMode = destState;
                        DeviceLiveData.ControlState = (int)CurControlMode;
                        DeviceLiveDataHandler.SaveAsync(DeviceLiveData);
                        return OperateResult.CreateSuccessResult();
                    }
                    else
                    {
                        return businessEnable;
                    }
                }
                else
                {
                    return controlEnable;
                }
            }
            else
            {
                return check;
            }
        }

        #endregion

    }
}
