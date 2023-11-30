using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    public abstract class InCheckWorkerAbstract : InCheckWorkerAbstractForT<List<string>>
    {

        private IdentifyWorkerProperty _workerProperty = new IdentifyWorkerProperty();

        public IdentifyWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }

        public override OperateResult GetWorkerRealData()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetWorkerRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }


        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            IdentityWorkerViewModel viewModel = new IdentityWorkerViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;

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
            IdentifyWorkerDetailView view = new IdentifyWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        protected OperateResult SendContentToScreen(string content)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            List<AssistantDevice> displayDevices = GetAssistantByDeviceType(DeviceTypeEnum.DisplayDevice);
            if (displayDevices == null || displayDevices.Count <= 0)
            {
                string msg = string.Format("发送屏显信息：{0} 失败，原因：找不到任何显示设备", content);
                LogMessage(msg, EnumLogLevel.Error, true);
                result.Message = msg;
                result.IsSuccess = false;
                return result;
            }
            foreach (AssistantDevice  assistant in displayDevices)
            {
                DisplayDeviceAbstract displayDevice = assistant.Device as DisplayDeviceAbstract;
                if (displayDevice == null)
                {
                    result.IsSuccess = false;
                    break;
                }
                OperateResult sendResult = displayDevice.SendContent(content);
                LogMessage(sendResult.Message, EnumLogLevel.Info, false);
                if (!sendResult.IsSuccess)
                {
                    LogMessage(string.Format("发送屏显：{0} 失败", content), EnumLogLevel.Warning, true);
                    return sendResult;
                }
                result.IsSuccess = true;
            }
            return result;
        }


        protected internal InCheckWorkerBusinessAbstract WorkerBusiness { get; set; }
        public override void HandleIdentifyReady(DeviceName deviceName, int newValue)
        {
            if (newValue < 0)
            {
                List<AssistantDevice> identifyDevices = GetAssistantByDeviceType(DeviceTypeEnum.IdentityDevice);
                if (identifyDevices != null && identifyDevices.Count > 0)
                {
                    foreach (AssistantDevice assistant in identifyDevices)
                    {
                        IdentifyDevice identifyDevice = assistant.Device as IdentifyDevice;
                        if (identifyDevice != null)
                        {
                            identifyDevice.GetIdentifyMessageAsync();
                        }
                    }
                    LogMessage(string.Format("收到就绪：{0} 触发读取条码成功", newValue), EnumLogLevel.Info, true);
                }
                else
                {
                    LogMessage(string.Format("收到就绪：{0}，但是找不到任何信息识别设备", newValue), EnumLogLevel.Info, true);
                }
            }
        }

        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            InCheckWorkerBusinessAbstract tempBusiness = business as InCheckWorkerBusinessAbstract;
            if (tempBusiness == null)
            {
                string msg = string.Format("协助者类转换出错，期望类型：IdentifyWorkerBusinessAbstract 实际类型：{0}", business.GetType());
                return OperateResult.CreateFailedResult(msg, 1);
            }
            this.WorkerBusiness = tempBusiness;
           
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularStart()
        {
            OperateResult regesteResult = RegisteHandler();
            if (!regesteResult.IsSuccess)
            {
                return regesteResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult RegisteHandler()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                foreach (AssistantDevice assistant in WorkerAssistants)
                {
                    if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.IdentityDevice))
                    {
                        IdentityDeviceAbstract<List<string>> identifyDevice = assistant.Device as IdentityDeviceAbstract<List<string>>;
                        if (identifyDevice != null)
                        {
                            identifyDevice.IdentifyMsgCallbackHandler += HandleIdentifyMsg;
                        }
                    }
                    if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.LoadDevice))
                    {
                        StationDeviceAbstract stationDevice = assistant.Device as StationDeviceAbstract;
                        if (stationDevice != null)
                        {
                            stationDevice.HandleOrderValueChangeEvent += HandleIdentifyReady;
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

                string devicePropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(devicePropertyXml))
                {
                    try
                    {
                        WorkerProperty = (IdentifyWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(IdentifyWorkerProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", devicePropertyXml));
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

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<InCheckWorkerAbstract, IdentifyWorkerProperty> viewModel = new WorkerConfigViewModel<InCheckWorkerAbstract, IdentifyWorkerProperty>(this, WorkerProperty);
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

                OperateResult initWorkerConfig= this.InitConfig();
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


    }

}
