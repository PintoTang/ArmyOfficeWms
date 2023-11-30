using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Framework.Log.Helper;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model
{
    /// <summary>
    /// 设备基类
    /// </summary>
    public abstract class DeviceBaseAbstract : ViewModelBase, IRestore, IManageable
    {
        #region 字段
        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        private bool _isHasError;
        private bool _isHasWarning;
        private string _exposedUnId;

        #endregion

        #region 属性
        /// <summary>
        /// 实时数据操作
        /// </summary>
        protected LiveDataAbstract LiveDataDbHelper { get; set; }

        /// <summary>
        /// 标识设备是否存在错误
        /// </summary>
        public bool IsHasError
        {
            get { return _isHasError; }
            set
            {
                _isHasError = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 标识设备是否存在警告
        /// </summary>
        public bool IsHasWarning
        {
            get { return _isHasWarning; }
            set
            {
                _isHasWarning = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 是否存在任务
        /// </summary>
        /// <returns></returns>
        public bool IsHasTask
        {
            get { return _isHasTask; }
            set
            {
                _isHasTask = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 当前地址
        /// </summary>
        public Addr CurAddress { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceTypeEnum DeviceType { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }

        public int WorkSize { get; set; }

        public string ExposedUnId
        {
            get
            {
                if (string.IsNullOrEmpty(_exposedUnId))
                {
                    return this.Id.ToString();
                }
                return _exposedUnId;

            }
            set { _exposedUnId = value; }
        }

        /// <summary>
        /// 设备标识名
        /// </summary>
        public DeviceName DeviceName { get; set; }

        private Pool<INotifyAttributeChange> _attributeListener = new Pool<INotifyAttributeChange>();
        private bool _isHasTask;


        /// <summary>
        /// 设置是否需要显示界面
        /// </summary>
        public bool IsShowUi { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="name"></param>
        /// <param name="deviceType"></param>
        /// <param name="currAddress"></param>
        /// <param name="workSize"></param>
        /// <param name="business"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public OperateResult Initialize<TDeviceBusiness, TDeviceControl>(int deviceId, DeviceName deviceName, string name, DeviceTypeEnum deviceType, Addr currAddress, int workSize, TDeviceBusiness business, TDeviceControl control)
            where TDeviceBusiness : DeviceBusinessBaseAbstract
            where TDeviceControl : DeviceControlBaseAbstract
        {

            this.Id = deviceId;
            this.DeviceName = deviceName;
            this.Name = name;
            this.DeviceType = deviceType;
            CurAddress = currAddress;
            WorkSize = workSize;
            LiveDataDbHelper = DependencyHelper.GetService<LiveDataAbstract>();
            OperateResult controlInit = control.Initailize(deviceId, deviceName);
            if (!controlInit.IsSuccess)
            {
                return controlInit;
            }
            control.LogMessageAction += LogMessage;
            OperateResult businessInit = business.Initialize(deviceId, deviceName);
            if (!businessInit.IsSuccess)
            {
                return businessInit;
            }
            business.LogMessageAction += LogMessage;
            OperateResult particaluarResult = ParticularInitlize(business, control);
            if (!particaluarResult.IsSuccess)
            {
                return particaluarResult;
            }
            OperateResult initConfigResult = InitConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }
            OperateResult startResult = Start();
            if (!startResult.IsSuccess)
            {
                return startResult;
            }
            return OperateResult.CreateSuccessResult();

        }

        private OperateResult Start()
        {
            OperateResult particularStartResult = ParticularStart();
            if (!particularStartResult.IsSuccess)
            {
                return particularStartResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        public abstract OperateResult ParticularStart();
        public abstract OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control);

        public abstract OperateResult ParticularConfig();

        private OperateResult InitConfig()
        {
            OperateResult particularConfig = ParticularConfig();

            return particularConfig;
        }

        /// <summary>
        /// 注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterAttributeListener(INotifyAttributeChange listener)
        {
            _attributeListener.AddPool(listener);
        }

        /// <summary>
        /// 解注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void UnRegisterAttributeListener(INotifyAttributeChange listener)
        {
            _attributeListener.RemovePool(listener);
        }

        /// <summary>
        /// 通知属性变化
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="NewValue">属性新值</param>
        public void NotifyAttributeChange(string attributeName, object NewValue)
        {
            var Task = System.Threading.Tasks.Task.Run(() =>
            {
                lock (_attributeListener)
                {
                    foreach (INotifyAttributeChange listener in _attributeListener.Container)
                    {
                        listener.NotifyAttributeChange(attributeName, NewValue);
                    }
                }
            });
        }


        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            if (level.Equals(EnumLogLevel.Error))
            {
                IsHasError = true;
            }
            else if (level.Equals(EnumLogLevel.Warning))
            {
                IsHasWarning = true;
            }
            else
            {
                IsHasError = false;
                IsHasWarning = false;
            }
            LogHelper.WriteLog(this.Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }
        /// <summary>
        /// 通过数据库获取设备的实时数据
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult GetDeviceRealStatus();
        public abstract OperateResult GetDeviceRealData();
        public OperateResult Restore()
        {

            OperateResult getStatus = GetDeviceRealStatus();
            if (!getStatus.IsSuccess)
            {
                return getStatus;
            }
            OperateResult getData = GetDeviceRealData();
            if (!getData.IsSuccess)
            {
                return getData;
            }
            return OperateResult.CreateSuccessResult();

        }
        public abstract OperateResult Availabe();

        public abstract OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr);

        /// <summary>
        /// 可以作为指令执行的起点的
        /// </summary>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        public abstract bool Accessible(Addr destAddr);

        public virtual bool IsTransportDevice()
        {
            if (this.DeviceType.Equals(DeviceTypeEnum.TransportDevice))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsEndDevice()
        {
            if (DeviceType.Equals(DeviceTypeEnum.RackPlace))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual OperateResult HandleRestoreData()
        {
            OperateResult restoreResult = Restore();
            if (!restoreResult.IsSuccess)
            {
                return restoreResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 设备清除相关运行的信息 当设备接收到指令后 清除相关设备信息
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult ClearUpRunMessage(TransportMessage transport);

        public abstract OperateResult<WareHouseViewModelBase> CreateViewModel();

        public OperateResult<WareHouseViewModelBase> GetViewModel()
        {
            OperateResult<WareHouseViewModelBase> findViewModel = ViewModelManage.Instance.FindWorkerViewModel(this.Id);
            if (findViewModel.IsSuccess)
            {
                return findViewModel;
            }
            try
            {
                OperateResult<WareHouseViewModelBase> newViewModel = CreateViewModel();
                if (newViewModel.IsSuccess)
                {
                    newViewModel.Content.ViewModelType = ViewModelTypeEnum.Device;
                    ViewModelManage.Instance.Add(newViewModel.Content);
                }
                findViewModel.Content = newViewModel.Content;
                findViewModel.IsSuccess = true;
                findViewModel.ErrorCode = 0;
            }
            catch (Exception ex)
            {

                findViewModel.IsSuccess = false;
                findViewModel.Message = OperateResult.ConvertException(ex);

            }

            return findViewModel;
        }

        protected abstract OperateResult<ViewAbstract> CreateView();

        public OperateResult<ViewAbstract> GetViewForWpf()
        {
            OperateResult<ViewAbstract> viewResult = new OperateResult<ViewAbstract>();
            OperateResult<WareHouseViewModelBase> viewModel = GetViewModel();
            if (!viewModel.IsSuccess)
            {
                viewResult.IsSuccess = false;
                return viewResult;
            }
            viewResult = CreateView();
            if (!viewResult.IsSuccess)
            {
                return viewResult;
            }
            viewResult.Content.DataContext = viewModel.Content;
            return viewResult;
        }

        protected abstract Window CreateAssistantView();

        public Window GetAssistantView()
        {
            OperateResult<WareHouseViewModelBase> viewModel = GetViewModel();
            if (!viewModel.IsSuccess)
            {
                return null;
            }
            Window assistantView = CreateAssistantView();
            if (assistantView == null) return null;
            assistantView.DataContext = viewModel.Content;
            return assistantView;
        }

        protected abstract Window CreateConfigView();

        public Window GetConfigView()
        {
            Window configView = CreateConfigView();
            return configView;
        }

        protected abstract OperateResult<UserControl> CreateDetailView();

        protected abstract OperateResult<UserControl> CreateMonitorView();

        public UserControl GetDetailViewForWpf()
        {
            OperateResult<UserControl> viewResult = new OperateResult<UserControl>();
            OperateResult<WareHouseViewModelBase> viewModel = GetViewModel();
            if (!viewModel.IsSuccess)
            {
                viewResult.IsSuccess = false;
                return null;
            }
            viewResult = CreateDetailView();
            if (!viewResult.IsSuccess)
            {
                return null;
            }
            viewResult.Content.DataContext = viewModel.Content;
            return viewResult.Content;
        }

        public UserControl GetMonitorViewForWpf()
        {
            OperateResult<UserControl> viewResult = new OperateResult<UserControl>();
            OperateResult<WareHouseViewModelBase> viewModel = GetViewModel();
            if (!viewModel.IsSuccess)
            {
                viewResult.IsSuccess = false;
                return null;
            }
            viewResult = CreateMonitorView();
            if (!viewResult.IsSuccess)
            {
                return null;
            }
            viewResult.Content.DataContext = viewModel.Content;
            return viewResult.Content;
        }

        /// <summary>
        /// 更新设备的属性值
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult UpdateProperty();

        public OperateResult SaveLiveDataToDb(LiveData data)
        {
            OperateResult saveResult = LiveDataDbHelper.Save(data);
            if (!saveResult.IsSuccess)
            {
                string logmsg = string.Format("保存当前数据失败 值：{0} 序号：{1}", data.DataValue, data.Index);
                LogMessage(logmsg, EnumLogLevel.Warning, true);
            }
            return saveResult;
        }

        public OperateResult RemoveLiveDataToDb(LiveData data)
        {
            OperateResult saveResult = LiveDataDbHelper.DeleteLiveData(data);
            if (!saveResult.IsSuccess)
            {
                string logmsg = string.Format("删除当前数据失败 值：{0} 序号：{1}", data.DataValue, data.Index);
                LogMessage(logmsg, EnumLogLevel.Warning, true);
            }
            return saveResult;
        }

        public OperateResult ClearLiveDataToDb()
        {
            OperateResult saveResult = LiveDataDbHelper.ClearLiveData(this.Id);
            if (!saveResult.IsSuccess)
            {
                string logmsg = "清除实时数据失败";
                LogMessage(logmsg, EnumLogLevel.Warning, true);
            }
            return saveResult;
        }

        public abstract void RefreshDeviceState();

        public override string ToString()
        {
            return Name;
        }

        #endregion

    }
}
