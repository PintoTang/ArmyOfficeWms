using System;
using System.Threading;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.TaskHandleCenter;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model
{
    public abstract class DeviceBaseForTask<TTask> : DeviceBaseAbstract, ITaskNotifyCentre<TTask> where TTask : IDeviceTaskContent
    {

        public override void RefreshDeviceState()
        {
            if (UnFinishedTask != null && UnFinishedTask.Lenght > 0)
            {
                IsHasTask = true;
            }
            else
            {
                IsHasTask = false;
            }
        }

        #region 设备状态

        protected LiveStatusAbstract DeviceLiveDataHandler;

        protected LiveStatusData DeviceLiveData = new LiveStatusData();
        private RunStateMode _curRunState;
        /// <summary>
        /// 运行模式
        /// </summary>
        public RunStateMode CurRunState
        {
            get { return _curRunState; }
            set
            {
                if (_curRunState != value)
                {
                    _curRunState = value;
                    RaisePropertyChanged("CurRunState");
                }
            }
        }

        private UseStateMode _curUseState = UseStateMode.Enable;
        private ConnectState _curConnectState = ConnectState.Online;
        public UseStateMode CurUseState
        {
            get { return _curUseState; }
            set
            {
                if (_curUseState != value)
                {
                    _curUseState = value;
                    RaisePropertyChanged("CurUseState");
                    //NotifyAttributeChange("CurUseState", value); //通知无效 待删除
                }
            }
        }

        private ControlStateMode _curControlMode = ControlStateMode.Auto;

        /// <summary>
        /// 当前的控制模式
        /// </summary>
        public ControlStateMode CurControlMode
        {
            get { return _curControlMode; }
            set
            {
                if (_curControlMode != value)
                {
                    _curControlMode = value;
                    NotifyAttributeChange("CurControlState", value);
                };
            }
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectState CurConnectState
        {
            get { return _curConnectState; }
            set
            {
                if (_curConnectState != value)
                {
                    _curConnectState = value;
                    RaisePropertyChanged("CurConnectState");
                }
            }
        }

        public bool SetDeviceStatus(UseStateMode stateMode)
        {
            this._curUseState = stateMode;
            return true;
        }

        public bool SetDeviceMode(RunStateMode runMode)
        {
            this._curRunState = runMode;
            return true;
        }


        #endregion
        /// <summary>
        /// 需要堆垛机Worker进行注册事件
        /// </summary>
        public Func<TaskExcuteMessage<TTask>, OperateResult> NotifyDeviceExceptionEvent;

      



        #region 通知任务执行状态
        private Pool<IHandleTaskExcuteStatus<TTask>> _taskExcuteStatusListenerList = new Pool<IHandleTaskExcuteStatus<TTask>>();
        public Pool<IHandleTaskExcuteStatus<TTask>> TaskExcuteStatusListenerList
        {
            get { return _taskExcuteStatusListenerList; }
            private set { _taskExcuteStatusListenerList = value; }
        }

        public OperateResult RegisterTaskExcuteStatusListener(IHandleTaskExcuteStatus<TTask> listener)
        {
            return _taskExcuteStatusListenerList.AddPool(listener);
        }

        public OperateResult UnRegisterTaskExcuteStatusListener(IHandleTaskExcuteStatus<TTask> listener)
        {
            return _taskExcuteStatusListenerList.RemovePool(listener);
        }

        public void NotifyTaskExcuteStatus(DeviceBaseAbstract finishDevice, TTask task, TaskExcuteStepStatusEnum excuteStepStatus)
        {
            lock (_taskExcuteStatusListenerList)
            {
                foreach (IHandleTaskExcuteStatus<TTask> listener in _taskExcuteStatusListenerList.Container)
                {
                    try
                    {
                        listener.HandleTaskExcuteStatus(finishDevice, task, excuteStepStatus);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(string.Format("通知：{0} 任务：{1} 执行状态:{2} 异常：{3}", listener.Name, task.UniqueCode, excuteStepStatus, OperateResult.ConvertExMessage(ex)), EnumLogLevel.Error, true);
                    }

                }
            }
        }

        #endregion
       

        public abstract void AddUnfinishedTask(TTask task);

        public abstract void RemoveUnfinishedTask(TTask order);

        public bool IsFull
        {
            get { return UnFinishedTask.Lenght >= WorkSize; }
        }

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

        private UniqueDataObservablePool<TTask> _unFinishedTask = new UniqueDataObservablePool<TTask>();

        public UniqueDataObservablePool<TTask> UnFinishedTask
        {
            get { return _unFinishedTask; }
            set
            {
                _unFinishedTask = value;
                RefreshDeviceState();
            }
        }


        /// <summary>
        /// 运行
        /// </summary>
        /// <returns></returns>
        public virtual OperateResult Run()
        {
            CurRunState = RunStateMode.Run;
            DeviceLiveData.Id = this.Id;
            DeviceLiveData.RunState = (int)CurRunState;
            DeviceLiveDataHandler.Save(DeviceLiveData);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public virtual OperateResult Pause()
        {
            CurRunState = RunStateMode.Pause;
            DeviceLiveData.Id = this.Id;
            DeviceLiveData.RunState = (int)CurRunState;
            DeviceLiveDataHandler.Save(DeviceLiveData);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>

        public virtual OperateResult Reset()
        {
            CurRunState = RunStateMode.Reset;
            //处理复位的操作
            Thread.Sleep(2000);
            CurRunState = RunStateMode.Pause;
            return OperateResult.CreateSuccessResult();
        }


        public abstract OperateResult SetAbleState(UseStateMode destState);


        public abstract OperateResult SetControlState(ControlStateMode destState);


        public OperateResult SetDispatchState(DispatchState destState)
        {
            OperateResult isCanChange = IsCanChangeDispatchState(destState);
            if (isCanChange.IsSuccess)
            {
                CurDispatchState = destState;
                DeviceLiveData.Id = this.Id;
                DeviceLiveData.DispatchState = (int)CurDispatchState;
                DeviceLiveDataHandler.Save(DeviceLiveData);
                return OperateResult.CreateSuccessResult();
            }
            else
            {
                return isCanChange;
            }
        }

        public abstract OperateResult IsCanChangeDispatchState(DispatchState destState);


        /// <summary>
        /// 调度状态
        /// </summary>
        public DispatchState CurDispatchState
        {
            get { return _curDispatchState; }
            set
            {
                if (_curDispatchState != value)
                {
                    _curDispatchState = value;
                    RaisePropertyChanged("CurDispatchState");
                }
            }
        }

        private DispatchState _curDispatchState = DispatchState.On;



        /// <summary>
        /// 设备当前是否可用
        /// </summary>
        /// <returns></returns>
        public override OperateResult Availabe()
        {
            if (_curConnectState.Equals(ConnectState.Offline))
            {
                return OperateResult.CreateFailedResult("处于离线状态");
            }
            if (!_curRunState.Equals(RunStateMode.Run))
            {
                return OperateResult.CreateFailedResult("处于非运行状态");
            }
            if (_curUseState.Equals(UseStateMode.Disable))
            {
                return OperateResult.CreateFailedResult("处于禁用状态");
            }
            if (_curDispatchState.Equals(DispatchState.Off))
            {
                return OperateResult.CreateFailedResult("处于非调度状态");
            }
            if (!_curControlMode.Equals(ControlStateMode.Auto))
            {
                return OperateResult.CreateFailedResult("处于非自动状态");
            }
            if (IsFull)
            {
                return OperateResult.CreateFailedResult("处于满负荷运行状态");
            }
            return OperateResult.CreateSuccessResult();
        }


    }
}
