using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common
{
    /// <summary>
    /// 工作者虚拟类
    /// </summary>
    public abstract class WorkerBaseAbstract : ViewModelBase, IRestore, IManageable
    {
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


        private int _workSize = 1;

        /// <summary>
        /// 工作区大小，能同时处理任务的个数
        /// </summary>
        public int WorkSize
        {
            get { return _workSize; }
            set { _workSize = value; }
        }

        /// <summary>
        /// 工作者的唯一编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 类型 Coordination Type="RollerLine"
        /// </summary>
        public WorkerTypeEnum WorkerType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public DeviceName WorkerName { get; protected internal set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }

        public OperateResult Initailize(int workerId, string name, WorkerTypeEnum workerType, int workSize, DeviceName workerName, WorkerBusinessAbstract business, string className, string nameSpace)
        {
            this.Id = workerId;
            this.WorkerName = workerName;
            this.Name = name;
            this.WorkerType = workerType;
            WorkSize = workSize;
            this.ClassName = className;
            this.NameSpace = nameSpace;
            OperateResult businessInit = business.Initialize(workerId, workerName);
            if (!businessInit.IsSuccess)
            {
                return businessInit;
            }
            business.LogMessageAction += LogMessage;
            OperateResult particaluarResult = ParticularInitlize(workerId, workerName, business);
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

        protected abstract OperateResult ParticularStart();

        private OperateResult InitAssistants()
        {
            _workerAssistants.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                string path = "Config/Assistants";
                XmlOperator doc = ConfigHelper.GetCoordinationConfig;
                XmlElement xmlElement = doc.GetXmlElement("Coordination", "Id", Id.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    result.Message = string.Format("通过组件：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }
                string devicesPropertyXml = xmlNode.OuterXml;
                AssistantsProperty devicesProperty = null;
                using (StringReader sr = new StringReader(devicesPropertyXml))
                {
                    try
                    {
                        devicesProperty = (AssistantsProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(AssistantsProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || devicesProperty == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：DevicesProperty", devicesPropertyXml));
                }

                foreach (AssistantProperty deviceProperty in devicesProperty.AssistantList)
                {
                    DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(deviceProperty.DeviceId);
                    if (device == null)
                    {
                        continue;
                    }
                    AssistantDevice assistant = new AssistantDevice(device, deviceProperty.AssistantType, deviceProperty.IsRegisterFinish);
                    AddWorkerAssistant(assistant);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }


        protected internal OperateResult InitConfig()
        {
            OperateResult initAssistants = InitAssistants();
            if (!initAssistants.IsSuccess)
            {
                return initAssistants;
            }
            return ParticularConfig();
        }
        protected abstract OperateResult ParticularConfig();

        public abstract OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business);

        private List<AssistantDevice> _workerAssistants = new List<AssistantDevice>();
        /// <summary>
        /// 工作者的装备助手们
        /// </summary>
        public List<AssistantDevice> WorkerAssistants
        {
            get { return _workerAssistants; }
            set
            {
                lock (_workerAssistants)
                {
                    _workerAssistants = value;
                }
            }
        }
        /// <summary>
        /// 移除指定的工作者的协助者
        /// </summary>
        /// <param name="assistant"></param>
        /// <returns></returns>
        public bool RemoveWorkerAssistant(AssistantDevice assistant)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(a => a.Id.Equals(assistant.Id)))
                {
                    AssistantDevice device = _workerAssistants.FirstOrDefault(a => a.Id.Equals(assistant.Id));
                    bool remove = _workerAssistants.Remove(device);
                    if (remove)
                    {
                        return true;
                    }
                    //打印移除协助者失败，把协助者信息打印出
                    return false;
                }
                return false;
            }
        }


        public bool AddWorkerAssistant(AssistantDevice assistant)
        {
            //添加工作者的设备的装备信息时，添加监听任务完成信息
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(a => a.Id.Equals(assistant.Id)))
                {
                    return true;
                }
                _workerAssistants.Add(assistant);
                return true;
            }
        }

        /// <summary>
        /// 通过协助者的ID获取指定的协助者
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssistantDevice GetAssistantById(int id)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Id.Equals(id)))
                {
                    return _workerAssistants.FirstOrDefault(e => e.Id.Equals(id));
                }
                return null;
            }
        }

        /// <summary>
        /// 通过协助者的ID获取指定的协助者
        /// </summary>
        /// <param name="exporedUnid"></param>
        /// <returns></returns>
        public AssistantDevice GetAssistantByExporedId(string exporedUnid)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Device.ExposedUnId.Equals(exporedUnid)))
                {
                    return _workerAssistants.FirstOrDefault(e => e.Device.ExposedUnId.Equals(exporedUnid));
                }
                return null;
            }
        }

        /// <summary>
        /// 通过设备类别查找所有的设备
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public List<AssistantDevice> GetAssistantByDeviceType(DeviceTypeEnum deviceType)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Device.DeviceType.Equals(deviceType)))
                {
                    return _workerAssistants.FindAll(e => e.Device.DeviceType.Equals(deviceType));
                }
                return null;
            }
        }
        /// <summary>
        /// 通过设备类别和当前的地址获取协助者
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="curAddr"></param>
        /// <returns></returns>
        public List<AssistantDevice> GetAssistantByDeviceTypeAndAddress(DeviceTypeEnum deviceType, Addr curAddr)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Device.DeviceType.Equals(deviceType) && e.Device.CurAddress.Equals(curAddr)))
                {
                    return _workerAssistants.FindAll(e => e.Device.DeviceType.Equals(deviceType) && e.Device.CurAddress.Equals(curAddr));
                }
                return null;
            }
        }

        /// <summary>
        /// 通过设备类别和当前的地址获取协助者
        /// </summary>
        /// <param name="curAddr"></param>
        /// <returns></returns>
        public List<AssistantDevice> GetAssistantByAddress(Addr curAddr)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Device.CurAddress.Equals(curAddr)))
                {
                    return _workerAssistants.FindAll(e => e.Device.CurAddress.Equals(curAddr));
                }
                return null;
            }
        }

        public AssistantDevice GetAssistantByDeviceName(DeviceName deviceName)
        {
            lock (_workerAssistants)
            {
                if (_workerAssistants.Exists(e => e.Device.DeviceName.Equals(deviceName)))
                {
                    return _workerAssistants.FirstOrDefault(e => e.Device.DeviceName.Equals(deviceName));
                }
                return null;
            }
        }

        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(this.Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }



        public OperateResult Restore()
        {
            OperateResult getStatus = GetWorkerRealStatus();
            if (!getStatus.IsSuccess)
            {
                return getStatus;
            }
            OperateResult getData = GetWorkerRealData();
            if (!getData.IsSuccess)
            {
                return getData;
            }
            return OperateResult.CreateSuccessResult();
        }

        public abstract OperateResult GetWorkerRealStatus();
        public abstract OperateResult GetWorkerRealData();

        private Pool<INotifyAttributeChange> attributeListener = new Pool<INotifyAttributeChange>();
        private bool _isHasError;
        private bool _isHasWarning;
        private bool _isHasTask;

        /// <summary>
        /// 注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterAttributeListener(INotifyAttributeChange listener)
        {
            attributeListener.AddPool(listener);
        }

        /// <summary>
        /// 解注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void UnRegisterAttributeListener(INotifyAttributeChange listener)
        {
            attributeListener.RemovePool(listener);
        }

        /// <summary>
        /// 通知属性变化
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="NewValue">属性新值</param>
        public void NotifyAttributeChange(string attributeName, object NewValue)
        {
            lock (attributeListener)
            {
                foreach (INotifyAttributeChange listener in attributeListener.Container)
                {
                    listener.NotifyAttributeChange(attributeName, NewValue);
                }
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

        protected abstract OperateResult<WareHouseViewModelBase> CreateViewModel();

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
                    newViewModel.Content.ViewModelType = ViewModelTypeEnum.Woker;
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

        protected abstract Window CreateConfigView();

        public Window GetConfigView()
        {
            Window configView = CreateConfigView();
            return configView;
        }

        protected internal abstract OperateResult UpdateProperty();

        protected abstract OperateResult<ViewAbstract> CreateView();

        protected abstract OperateResult<UserControl> CreateDetailView();

        protected abstract OperateResult<UserControl> CreateMonitorView();

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

        public virtual void RefreshWorkerState()
        {
            return;
        }

    }
}
