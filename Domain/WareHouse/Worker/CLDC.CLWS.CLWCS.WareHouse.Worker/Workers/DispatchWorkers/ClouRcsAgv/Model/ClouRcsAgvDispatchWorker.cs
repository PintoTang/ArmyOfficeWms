using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.View;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    /// <summary>
    /// 杭叉AGV调度的工作逻辑
    /// </summary>
    public class ClouRcsAgvDispatchWorker : DispatchWorkerAbstract<ClouRcsAgvWorkerBusiness>
    {

        private ClouRcsAgvDispatchWorkerProperty _workerProperty = new ClouRcsAgvDispatchWorkerProperty();

        public new ClouRcsAgvDispatchWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }

        public OperateResult NotifyDeviceStatusHandler(int wcsAgvId, int onlineStatus, int expStatus, int expCode)
        {
            AssistantDevice assistant = GetAssistantById(wcsAgvId);
            if (assistant == null)
            {
                string msg = string.Format("根据设备编号:{0} 找不到对应的设备", wcsAgvId);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            AgvDeviceAbstract agvDevice = assistant.Device as AgvDeviceAbstract;
            if (agvDevice == null)
            {
                string msg = string.Format("设备：{0} 不是Agv设备，请检查设备编号是否配置正确", assistant.Device.Name + assistant.Device.Id);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            agvDevice.HandleDeviceChange(agvDevice.DeviceName, DeviceChangeModeEnum.OnlineStatus, onlineStatus);
            agvDevice.HandleDeviceChange(agvDevice.DeviceName, DeviceChangeModeEnum.ExcpetionStatus, expStatus);
            if (expStatus.Equals(1))
            {
                agvDevice.HandleDeviceChange(agvDevice.DeviceName, DeviceChangeModeEnum.ExcpetionStatus, expCode);
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult NotifyExeResultEventHandler(int wcsAgvId, int exeStatus, string taskNo)
        {
            AssistantDevice assistant = GetAssistantById(wcsAgvId);
            if (assistant == null)
            {
                string msg = string.Format("根据设备编号:{0} 找不到对应的设备", wcsAgvId);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            AgvDeviceAbstract agvDevice = assistant.Device as AgvDeviceAbstract;
            if (agvDevice == null)
            {
                string msg = string.Format("设备：{0} 不是Agv设备，请检查设备编号是否配置正确", assistant.Device.Name + assistant.Device.Id);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            AgvMoveStepEnum step = (AgvMoveStepEnum)exeStatus;
            int orderId = int.Parse(taskNo);
            agvDevice.HandleMoveStepChange(orderId, step);
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig; 
                XmlNode xmlNode = doc.GetXmlNode("Coordination", "Id", Id.ToString());

                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string workerConfigXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(workerConfigXml))
                {
                    try
                    {
                        WorkerProperty = (ClouRcsAgvDispatchWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(ClouRcsAgvDispatchWorkerProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：OrderWorkerConfigProperty", workerConfigXml));
                }

                foreach (PrefixsItem addrPrefix in WorkerProperty.Config.AddrPrefixs.AddrPrefixsList)
                {
                    if (addrPrefix.Type.Equals("In"))
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddInSrcAddrPrefixList(addrLst[i]);
                        }
                    }
                    else if (addrPrefix.Type.Equals("Out"))
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddOutSrcAddrPrefixList(addrLst[i]);
                        }
                    }
                    else
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddMoveSrcAddrPrefixList(addrLst[i].Trim());
                        }
                    }
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

        protected override OperateResult ParticularStart()
        {
            OperateResult baseStart = base.ParticularStart();
            if (!baseStart.IsSuccess)
            {
                return baseStart;
            }

            OperateResult registerResult = RegisterHandler();
            if (!registerResult.IsSuccess)
            {
                return registerResult;
            }
            return OperateResult.CreateSuccessResult();
        }
        public OperateResult RegisterHandler()
        {
            foreach (AssistantDevice assistant in WorkerAssistants)
            {
                if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.TransportDevice))
                {
                    AgvDeviceAbstract agvDevice = assistant.Device as AgvDeviceAbstract;
                    if (agvDevice != null)
                    {
                        WorkerBusiness.NotifyDeviceStatusEvent += NotifyDeviceStatusHandler;
                        WorkerBusiness.NotifyExeResultEvent += NotifyExeResultEventHandler;
                    }
                }
            }
            return OperateResult.CreateSuccessResult();
        }
        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            ClouRcsAgvWorkerViewModel viewModel = new ClouRcsAgvWorkerViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<ClouRcsAgvDispatchWorker, ClouRcsAgvDispatchWorkerProperty> viewModel = new WorkerConfigViewModel<ClouRcsAgvDispatchWorker, ClouRcsAgvDispatchWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected internal override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = WorkerProperty.Name;
                this.WorkSize = WorkerProperty.WorkSize;
                this.WorkerName = new DeviceName(WorkerProperty.WorkerName);
                this.NameSpace = WorkerProperty.NameSpace;
                this.ClassName = WorkerProperty.ClassName;
                this.Id = WorkerProperty.WorkerId;

                this.WorkerBusiness.Name = WorkerProperty.BusinessHandle.Name;
                this.WorkerBusiness.ClassName = WorkerProperty.BusinessHandle.ClassName;
                this.WorkerBusiness.NameSpace = WorkerProperty.BusinessHandle.NameSpace;

                OperateResult initWorkerConfig = this.InitConfig();
                if (!initWorkerConfig.IsSuccess)
                {
                    return initWorkerConfig;
                }

                OperateResult initBusinessConfig = this.WorkerBusiness.InitConfig();
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
        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            WorkerShowCard view = new WorkerShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            OrderWorkerDetailView view = new OrderWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> type)
        {
            return OperateResult.CreateSuccessResult();
        }

       
    }
}
