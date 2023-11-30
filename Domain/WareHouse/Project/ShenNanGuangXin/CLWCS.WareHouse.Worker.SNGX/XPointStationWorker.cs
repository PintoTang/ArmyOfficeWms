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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.ViewModel;
using CLDC.Infrastructrue.Xml;


namespace CLWCS.WareHouse.Worker.HeFei
{
    public class XPointStationWorker : SwtichingWorkerAbstract<SwitchingWorkerBusinessAbstract>
    {

        private SwitchingWorkerProperty _workerProperty = new SwitchingWorkerProperty();

        public SwitchingWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }

        public override OperateResult SwithValueChangeHandle(object sender, int newValue)
        {
            //处理按钮设备
            List<AssistantDevice> switchDevices = GetAssistantByDeviceType(DeviceTypeEnum.SwitchDevice);
            if (switchDevices != null && switchDevices.Count > 0)
            {

                SwitchDevice switchDevice = sender as SwitchDevice;
                if (switchDevice != null)
                {
                    OperateResult switchHandle = WorkerBusiness.HandleValueChange(switchDevice, newValue);
                    if (switchHandle.IsSuccess)
                    {
                        LogMessage(switchHandle.Message, EnumLogLevel.Info, true);
                    }
                    else
                    {
                        LogMessage(switchHandle.Message, EnumLogLevel.Error, true);
                    }
                    return switchHandle;
                }
                LogMessage(string.Format("收到安全光栅/避障开关/安全门的信号变化：{0} 触发上报业务处理", newValue), EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(string.Format("收到就绪：{0}，但是找不到任何按钮设备", newValue), EnumLogLevel.Info, true);
            }
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularStart()
        {
            OperateResult registeResult = RegisteHandler();
            if (!registeResult.IsSuccess)
            {
                return registeResult;
            }
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

                string devicePropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(devicePropertyXml))
                {
                    try
                    {
                        WorkerProperty = (SwitchingWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(SwitchingWorkerProperty));
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

        public override OperateResult GetWorkerRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetWorkerRealData()
        {
            return OperateResult.CreateSuccessResult();
        }
        private OperateResult RegisteHandler()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                foreach (AssistantDevice assistant in WorkerAssistants)
                {
                    if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.SwitchDevice))
                    {
                        SwitchDeviceAbstract stationDevice = assistant.Device as SwitchDeviceAbstract;
                        if (stationDevice != null)
                        {
                            stationDevice.SwithValueChangeEvent += SwithValueChangeHandle;
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



        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            SwitchingWorkerDeviceViewModel viewModel = new SwitchingWorkerDeviceViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;

        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<XPointStationWorker, SwitchingWorkerProperty> viewModel = new WorkerConfigViewModel<XPointStationWorker, SwitchingWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult UpdateProperty()
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
            UCSwitchingWorkerDeviceView view = new UCSwitchingWorkerDeviceView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            SwitchingWorkerDetailView view = new SwitchingWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }
    }
}